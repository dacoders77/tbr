using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace TBR_noform
{
	/**
	 * Clears DB:
	 * UPDATE assets SET fx_quote = 0, fx_quote_processed = 0, volume = 0, order_placed = 0, info = "", request_id = null, stock_quote = 0
	 * Updates json in DB:
	 * update assets set info2 = JSON_SET(info2, "$.time", "123") where id = 85
	 * Overwrites json object:
	 * update assets set info2 = JSON_SET(info2, "$.time", JSON_OBJECT("long", "555")) where id = 85 
	 * 
	 * Watches and executes baskets. Calculates elapsed time for execution. 
	 * Stores all information in DB
	 * Steps:
	 * 1. Watch executed column in baskets table (tbr data base). If it is = 0 - execute basket. This behavoir is for debbuging only
	 * 1.1 Execution time > current time. Execute basket. Regular (not debbuging) behavoir
	 * 2. Get all assets from assets table with the same id
	 * 3. For each of this assets get forex quotes and write them to fx_quote column
	 * 4. 
	 * 
	 * 1. Time is up
	 * 2. Request forex quotes and writem them to DB. USD is not requested. 1 is written to fx quote without request
	 * 3. 
	 * 
	 * Each time new request id is written to request_id field. 
	 * 
	 * Request codes. All requests start with codes. This allows to determin the origin of the request when it is received in Form1.cs IbClient_TickPrice
	 * C# requests: 
	 * 5 - fx
	 * 6 - stock
	 * 8 - place order
	 * PHP:
	 * 7 - stock
	 *
	 */


	public class Basket
	{
		// Class variables
		private Form1 form;
		private MySqlConnection dbConn; // MySql connection variable
		private string connectionString;
		private string sqlQueryString;
		
		public bool isBasketExecuting; // Basket executing flag. When true - basket is executing right now and other qute requests from fron end will be rejected
		private int basketId;

		public double basketAllocatedFunds; // Funds for the basket
		public string basketAsset;

		public double assetQuote;
		public double assetForexQuote;
		public string assetCurrency;

		private double volume;

		// Class constructor
		public Basket(Form1 Form) {

			form = Form;
			isBasketExecuting = false; 
			connectionString = "server=" + Settings.dbHost + ";user id=slinger;password=659111;database=tbr";
			dbConn = new MySqlConnection(connectionString);
		}

		// Bastekts watch
		// Runs through all baskets in baskets table. When time is up - execute all assets from this basket and set execution flag of the basket to 1
		// When fx quotes are received and written to DB, basket.WatchFxQuoteRow() Form.cs line 353 picks them up and fire further events
		public void Watch() {

			using (var mySqlConnection = new MySqlConnection(connectionString))
			{
				if (mySqlConnection.State == System.Data.ConnectionState.Closed)
				{
					mySqlConnection.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("Basket.cs. Line 67. Connection state: " + dbConn.State + ". Connection is open, no need to connect");
				}

				sqlQueryString = "SELECT * from baskets"; // Get all baskets. Then check time on each
				MySqlCommand cmd4 = new MySqlCommand(sqlQueryString, mySqlConnection); // Use the connection opened in the constructor
				cmd4.ExecuteNonQuery();
				MySqlDataReader readerBasketsTable = cmd4.ExecuteReader();

				while (readerBasketsTable.Read())
				{
					// Calculate elapsed time and update eache record. Then it will be read with php and shown at the web form
					int id = Convert.ToInt32(readerBasketsTable["id"]); // Get id of the record

					DateTime executionTime = (DateTime)readerBasketsTable["execution_time"];
					TimeSpan timeSpan = executionTime.Subtract(DateTime.Now);

					// Calculate elapsed_time only for not executed baskets
					// Calculate elapsed time and update each record. Then it will be read with php and shown at the web form
					if (!(bool)readerBasketsTable["executed"])
					{
						basketId = Convert.ToInt32(readerBasketsTable["id"]);

						var sqlConnectionUpdateRecord = new MySqlConnection(connectionString);
						sqlConnectionUpdateRecord.Open();
						sqlQueryString = string.Format("UPDATE `baskets` SET `elapsed_time` = '" + timeSpan.Days.ToString() + ":" + timeSpan.Hours.ToString() + ":" + timeSpan.Minutes.ToString() + ":" + timeSpan.Seconds.ToString() + "' WHERE `baskets`.`id` = {0}", id);
						MySqlCommand sqlCommandUpdateRecord = new MySqlCommand(sqlQueryString, sqlConnectionUpdateRecord);
						sqlCommandUpdateRecord.ExecuteNonQuery();
						sqlConnectionUpdateRecord.Close();
					}



					// DEBUG is_executed = 0
					// Execution time is > current time. A basket needs to be executed
					if (!(bool)readerBasketsTable["executed"]) // This case is only when exectute == 0. Just for debbuing purpuse. Not to wait for execution time. Set it to 0 - basket is executed
					//if ((DateTime.Compare((DateTime)readerBasketsTable["execution_time"], DateTime.Now)) < 0 && !(bool)readerBasketsTable["executed"])
					{
						basketAllocatedFunds = Convert.ToDouble(readerBasketsTable["allocated_funds"]); // Get basket allocated funds
						var sqlConnectionSelectAssets = new MySqlConnection(connectionString);
						sqlConnectionSelectAssets.Open();

						sqlQueryString = string.Format("SELECT * from assets WHERE basket_id = {0}", basketId); 
						MySqlCommand cmd5 = new MySqlCommand(sqlQueryString, sqlConnectionSelectAssets);
						cmd5.ExecuteNonQuery();
						MySqlDataReader readerAssetsTable = cmd5.ExecuteReader();

						while (readerAssetsTable.Read()) // Loop throught the assets which must be executed. For each asset an order shall be places
						{
							int requestId = (int)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds(); // Make a randome number. Use it as a request id
							string x = "5" + requestId.ToString().Remove(0, 3); // Delete 2 characters from the beggining. Otherwise it is not gonna be accepted by IB
							requestId = Convert.ToInt32(x); // First character request codes: 5 - fx, 6 - stock. PHP: 7 - stock. Later this first character will be used to determine what type of response is it

							// CAD, USDCHF and USDJPY are quoted the same way. Upside down https://groups.io/g/twsapi/topic/22665204?p=Created,,,20,2,0,0
							if (readerAssetsTable["currency"].ToString() == "CAD" || readerAssetsTable["currency"].ToString() == "JPY" || readerAssetsTable["currency"].ToString() == "CHF") 
							{
								UpdateRequestId(requestId, (int)readerAssetsTable["basket_id"], readerAssetsTable["currency"].ToString());
								form.apiManager.GetForexQuote("USD", readerAssetsTable["currency"].ToString(), Convert.ToInt32(readerAssetsTable["id"]), requestId);
							}
							else if (readerAssetsTable["currency"].ToString() != "USD") // All other symbols
							{
								UpdateRequestId(requestId, (int)readerAssetsTable["basket_id"], readerAssetsTable["currency"].ToString()); // Add request to DB
								form.apiManager.GetForexQuote(readerAssetsTable["currency"].ToString(), "USD", Convert.ToInt32(readerAssetsTable["id"]), requestId);
								//form.apiManager.GetQuote(readerAssetsTable["symbol"].ToString(), 1, readerAssetsTable["currency"].ToString());
							}

							if (readerAssetsTable["currency"].ToString() == "USD") {
								// No need to request a qute. USD is allways = 1. Update fx_quote = 1
								UpdateUsdFxQuote((int)readerAssetsTable["basket_id"]);
							}

						}

						sqlConnectionSelectAssets.Close();

						// Set executed flag to true and update status to filled
						var sqlConnectionUpdateRecord = new MySqlConnection(connectionString);
						sqlConnectionUpdateRecord.Open();
						sqlQueryString = string.Format("UPDATE baskets SET executed=1, status='filled' WHERE id = {0}", id); // only strings are covered with '' quotes
						MySqlCommand sqlCommandUpdateRecord = new MySqlCommand(sqlQueryString, sqlConnectionUpdateRecord);
						sqlCommandUpdateRecord.ExecuteNonQuery();
						sqlConnectionUpdateRecord.Close();

					}

				} 
				readerBasketsTable.Close();
			} 
		}

		// Once a secod the whole fx_quote row is checked for the presance of a quote
		public void WatchFxQuoteRow()
		{
			//fx_quote_processed
			using (var mySqlConnection = new MySqlConnection(connectionString))
			{
				if (mySqlConnection.State == System.Data.ConnectionState.Closed)
				{
					mySqlConnection.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("Basket.cs. Line 182. Connection state: " + dbConn.State + " .Connection open, no need to connect");
				}

				// WHERE fx_quote != 0 AND fx_quote_processed = 0. When a record is found - make a stock quote request and set fx_quote_processed to false
				sqlQueryString = "SELECT * FROM assets WHERE fx_quote != 0 and fx_quote_processed = 0";
				
				MySqlCommand cmd4 = new MySqlCommand(sqlQueryString, mySqlConnection); 
				cmd4.ExecuteNonQuery();
				MySqlDataReader readerAssetsTable = cmd4.ExecuteReader();

				while (readerAssetsTable.Read())
				{
					int requestId = (int)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
					string x = "6" + requestId.ToString().Remove(0, 3); // Add 6 as the first character in the ID
					requestId = Convert.ToInt32(x); // C# requests: 5 - fx, 6 - stock. PHP: 7 - stock. 8 - place order

					UpdateFxQuoteStatus((int)readerAssetsTable["basket_id"], readerAssetsTable["symbol"].ToString(), requestId, readerAssetsTable["currency"].ToString());
				}
			}
		}

		public void WatchVolumeRow()
		{
			using (var mySqlConnection = new MySqlConnection(connectionString))
			{
				if (mySqlConnection.State == System.Data.ConnectionState.Closed)
				{
					mySqlConnection.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("Basket.cs. Line 217. Connection state: " + dbConn.State + " .Connection open, no need to connect");
				}

				sqlQueryString = "SELECT * FROM assets WHERE volume != 0 and order_placed = 0";

				MySqlCommand cmd4 = new MySqlCommand(sqlQueryString, mySqlConnection);
				cmd4.ExecuteNonQuery();
				MySqlDataReader readerAssetsTable = cmd4.ExecuteReader();

				
				while (readerAssetsTable.Read())
				{
					int requestId = (int)new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
					string x = "8" + requestId.ToString().Remove(0, 3);
					requestId = Convert.ToInt32(x); // C# requests: 5 - fx, 6 - stock. PHP: 7 - stock. 8 - order execution

					//MessageBox.Show("place order: " + readerAssetsTable["symbol"].ToString());
					UpdatePlaceOrderStatus((int)readerAssetsTable["basket_id"], readerAssetsTable["symbol"].ToString(), requestId, readerAssetsTable["currency"].ToString(), Convert.ToInt32(readerAssetsTable["volume"]));
				}
			}
		}

		public void addForexQuoteToDB(int requestId, double fxQuote)
		{

			using (var mySqlConnection = new MySqlConnection(connectionString))
			{
				if (mySqlConnection.State == System.Data.ConnectionState.Closed)
				{
					mySqlConnection.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("Basket.cs. Line 252. Connection state: " + dbConn.State + " .Connection open, no need to connect");
				}

				// Update FX quote in the DB using a request id
				// Add a log record that FX quote was successfully received 
				// There are three parts in this request:
				// 1. FX quote update
				// 2. status:ok update
				// 3. log update (using string concatination)

				// When updating json we need to send: object name (fxQuoteRequest, placeOrder etc.), log text.
				// This methos is ready: UpdateInfoJson 

				sqlQueryString = string.Format(

					" UPDATE assets SET fx_quote = {0}, info = JSON_SET(info, '$.fxQuoteRequest', JSON_SET(info->'$.fxQuoteRequest', '$.status', 'ok')), info = JSON_SET(info, '$.fxQuoteRequest.log', CONCAT(`info`->>'$.fxQuoteRequest.log', '{2} FX Quote seccessfully received. Request Id: {1}.<br> ')) WHERE request_id = {1} ", fxQuote, requestId, DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.fff"));


				//"UPDATE assets " +
				//"SET fx_quote = {0}, " +
				//"info = JSON_SET(info, '$.fxQuoteRequest', JSON_SET((json_extract(info, '$.fxQuoteRequest')), '$.status', 'ok'))  " +
				//"WHERE request_id = {1}", fxQuote, requestId);

				MySqlCommand updateFxQuote = new MySqlCommand(sqlQueryString, mySqlConnection); // Use the connection opened in the constructor
				updateFxQuote.ExecuteNonQuery();
				mySqlConnection.Close();

			}
		}

		private void UpdateRequestId(int requestId, int basketId, string currency) {

			using (var mySqlConnection = new MySqlConnection(connectionString))
			{
				if (mySqlConnection.State == System.Data.ConnectionState.Closed)
				{
					mySqlConnection.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("BasketWatch.cs. Line 216. Connection state: " + dbConn.State + " .Connection open, no need to connect");
				}

				//sqlQueryString = string.Format("UPDATE assets SET request_id = {0} WHERE basket_id = {1} AND currency = '{2}'", requestId, readerAssetsTable["basket_id"], readerAssetsTable["currency"]);
				sqlQueryString = string.Format("UPDATE assets SET request_id = {0} WHERE basket_id = {1} AND currency = '{2}'", requestId, basketId, currency);
				MySqlCommand updateRequestId = new MySqlCommand(sqlQueryString, mySqlConnection);
				updateRequestId.ExecuteNonQuery();
				mySqlConnection.Close();

			}

		}

		private void UpdateUsdFxQuote(int basketId)
		{

			using (var mySqlConnection = new MySqlConnection(connectionString))
			{
				if (mySqlConnection.State == System.Data.ConnectionState.Closed)
				{
					mySqlConnection.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("BasketWatch.cs. Line 288. Connection state: " + dbConn.State + " .Connection open, no need to connect");
				}

				sqlQueryString = string.Format("UPDATE assets SET fx_quote = 1 WHERE basket_id = {0} AND currency = 'USD'", basketId);
				MySqlCommand updateRequestId = new MySqlCommand(sqlQueryString, mySqlConnection);
				updateRequestId.ExecuteNonQuery();
				mySqlConnection.Close();
			}
		}


		private void UpdateFxQuoteStatus(int basketId, string symbol, int requestId, string currency)
		{

			using (var mySqlConnection = new MySqlConnection(connectionString))
			{
				if (mySqlConnection.State == System.Data.ConnectionState.Closed)
				{
					mySqlConnection.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("BasketWatch.cs. Line 282. Connection state: " + dbConn.State + " .Connection open, no need to connect");
				}

				sqlQueryString = string.Format("UPDATE assets SET request_id = {0}, fx_quote_processed = 1 WHERE basket_id = {1} AND symbol = '{2}' ", requestId, basketId, symbol);
				MySqlCommand updateRequestId = new MySqlCommand(sqlQueryString, mySqlConnection);
				updateRequestId.ExecuteNonQuery();
				mySqlConnection.Close();

				form.apiManager.GetQuoteBasket(symbol, currency, requestId); // Get quote. The callback will be received in Form1.cs tickprice method
			}
		}

		private void UpdatePlaceOrderStatus(int basketId, string symbol, int requestId, string currency, int volume)
		{
			using (var mySqlConnection = new MySqlConnection(connectionString))
			{
				if (mySqlConnection.State == System.Data.ConnectionState.Closed)
				{
					mySqlConnection.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("BasketWatch.cs. Line 347. Connection state: " + dbConn.State + " .Connection open, no need to connect");
				}

				sqlQueryString = string.Format("UPDATE assets SET request_id = {0}, order_placed = 1 WHERE basket_id = {1} AND symbol = '{2}' ", requestId, basketId, symbol);
				MySqlCommand updateRequestId = new MySqlCommand(sqlQueryString, mySqlConnection);
				updateRequestId.ExecuteNonQuery();
				mySqlConnection.Close();

				// Place order
				form.apiManager.PlaceOrder(symbol, currency, volume, requestId);
			}
		}

		// When basket needs to be executed, a stock quote is requested in order to calculate volume
		public void UpdateStockQuoteValue(int requestId, double stockQuote, Form1 form) {

			// Update record in DB
			using (var mySqlConnection = new MySqlConnection(connectionString))
			{
				if (mySqlConnection.State == System.Data.ConnectionState.Closed)
				{
					mySqlConnection.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("BasketWatch.cs. Line 368. Connection state: " + dbConn.State + " .Connection open, no need to connect");
				}

				/*
				// Update request_id, status
				sqlQueryString = string.Format("UPDATE assets SET request_id = {0}, fx_quote_processed = 1 WHERE basket_id = {1} AND symbol = '{2}' ", requestId, basketId, "huy");
				MySqlCommand updateRequestId = new MySqlCommand(sqlQueryString, mySqlConnection);
				updateRequestId.ExecuteNonQuery();

				// Get basket ID. Delete it?
				sqlQueryString = string.Format("SELECT basket_id FROM assets WHERE request_id = {0}", requestId);
				MySqlCommand cmd6 = new MySqlCommand(sqlQueryString, mySqlConnection);
				object basketIdAssets = cmd6.ExecuteScalar();
				*/

				// Get basket allocated funds. Works good
				sqlQueryString = string.Format("SELECT allocated_funds FROM baskets WHERE id = {0}", basketId);
				MySqlCommand cmd4 = new MySqlCommand(sqlQueryString, mySqlConnection);
				object basketAllocatedFunds = cmd4.ExecuteScalar();

				// Get asset allocated percent
				sqlQueryString = string.Format("SELECT allocated_percent FROM assets WHERE request_id = {0}", requestId);
				MySqlCommand cmd5 = new MySqlCommand(sqlQueryString, mySqlConnection);
				object assetAllocatedPercent = cmd5.ExecuteScalar();

				// Get fx quote
				sqlQueryString = string.Format("SELECT fx_quote FROM assets WHERE request_id = {0}", requestId);
				MySqlCommand cmd6 = new MySqlCommand(sqlQueryString, mySqlConnection);
				object fxQuote = cmd6.ExecuteScalar();

				// VOLUME CALCULATION
				// Stock quote can be zero. In this case record zero to DB. This asset will not be executed.
				if (stockQuote != 0)
				{
					//vol = (10.000 * 13 % / 100) / 182 * 1.16
					volume = Math.Ceiling((Convert.ToDouble(basketAllocatedFunds) * Convert.ToDouble(assetAllocatedPercent) / 100) / stockQuote * Convert.ToDouble(fxQuote));
				}
				else
				{
					volume = 0;
					string info = DateTime.Now.ToString() + " Stock quote received as 0. Stock wont be executed";
					// Stock quote was recevied with 0 value. Add this info to DB and mark DB execution status as error (partial filled)
					sqlQueryString = string.Format("UPDATE assets SET info = '{0}' WHERE request_id = {1} ", info, requestId);
					MySqlCommand updateInfo = new MySqlCommand(sqlQueryString, mySqlConnection);
					updateInfo.ExecuteNonQuery();
				}

				
				sqlQueryString = string.Format("UPDATE assets SET volume = {0}, stock_quote = {1} WHERE request_id = {2} ", volume, stockQuote, requestId);
				MySqlCommand cmd7 = new MySqlCommand(sqlQueryString, mySqlConnection);
				cmd7.ExecuteNonQuery();
				

				//MessageBox.Show(Convert.ToDouble(result).ToString());
				ListViewLog.AddRecord(form, "brokerListBox", "Basket.cs", "Line 427. Volume calculated. Alloc. funds, stk quote, fx quote, req id:  " + Convert.ToDouble(basketAllocatedFunds).ToString() + " | " + stockQuote + " | " + Convert.ToDouble(fxQuote).ToString() + " | " + volume + " | " + requestId, "green");
				
				mySqlConnection.Close();
			}
		}

		// Updates JSON feild info in the DB. This method is called when making FX quote request, volume calculation, quote request etc.
		// Then the respose is received in 
		// Later, using this json data reports will be built. These reports are gonna be availible in the browser by clicking on filled icon in the basket list
		public void UpdateInfoJson(int id, int basketId, string symbol, int requestId, string currency, int volume)
		{
			string z = "fxQuoteRequest";

			using (var mySqlConnection = new MySqlConnection(connectionString))
			{
				if (mySqlConnection.State == System.Data.ConnectionState.Closed)
				{
					mySqlConnection.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("Basket.cs. Line 444. Connection state: " + dbConn.State + " .Connection open, no need to connect");
				}

				// sqlQueryString = string.Format(" UPDATE assets set info = JSON_SET(info, '$.fxQuoteRequest', JSON_SET((json_extract(info, '$.fxQuoteRequest')), '$.requestId', {0}, '$.time', '{1}'))  where id = {2} ", requestId, DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss:fff"), id, z);
				// sqlQueryString = string.Format(" UPDATE assets SET info = JSON_SET(info, '$.{3}', JSON_SET((json_extract(info, '$.{3}')), '$.requestId', {0}, '$.time', '{1}', '$.info', 'Request sent'))  where id = {2} ", requestId, DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss:fff"), id, z);

				sqlQueryString = string.Format(" UPDATE assets SET info = JSON_SET(info, '$.fxQuoteRequest.log', CONCAT(`info`->> '$.fxQuoteRequest.log', '{0} FX quote request sent. Request id: {1}.<br> ')) WHERE request_id = {1} ", DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.fff"), requestId);


				MySqlCommand updateRequestId = new MySqlCommand(sqlQueryString, mySqlConnection);
				updateRequestId.ExecuteNonQuery();
				mySqlConnection.Close();
			}
		}
	}
}
