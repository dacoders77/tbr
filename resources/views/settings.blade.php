@extends('layouts.app_tbr')

@section('content')

   <br>

   <div class="container text-center" style="border-style: solid; border-width: thin; border-color: transparent;">
      <div class="row align-items-center">
         <div  style="display: inline-block; width: 20%; border-style: solid; border-width: thin; border-color: transparent; font-size:2em;">
            <a style="color:Tomato" href="{{ url('/settings')}}">
               <i class="fas fa-sliders-h"></i>
            </a>
         </div>
         <div class="container col" style="border-style: solid; border-width: thin; border-color: transparent">
            <form>
               <div >
                  <button style="width: 100%" type="submit" class="btn btn-success"><i class="fas fa-play"></i>&nbsp;Start Bot</button>
               </div>
            </form>
         </div>
         <div style="display: inline-block; width: 20%; font-size:35px; color:Tomato">

         <!--
                            <a style="color:Tomato" href="{{ route('logout') }}">
                            <i class="fas fa-sign-out-alt"></i>
                            </a>
                            -->

            <a style="color:Tomato" class="dropdown-item" href="{{ route('logout') }}"
               onclick="event.preventDefault();
                                                     document.getElementById('logout-form').submit();">
               <i class="fas fa-sign-out-alt"></i>
            </a>

            <form id="logout-form" action="{{ route('logout') }}" method="POST" style="display: none;">
               @csrf
            </form>

         </div>
      </div>
   </div>

@endsection



