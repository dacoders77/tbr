@extends('layouts.app_tbr')

@section('content')
   <h2>Settings page2</h2>


   <div>
      <a href="{{ route('logout') }}"
         onclick="event.preventDefault(); document.getElementById('logout-form').submit();">
         Logout
      </a>

      <form id="logout-form" action="{{ route('logout') }}" method="POST" style="display: none;">
         @csrf
      </form>
   </div>

@endsection



