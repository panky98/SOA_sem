import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import logo from './logo.svg';
import './App.css';
import { HubConnectionBuilder } from '@microsoft/signalr';
import React, { useState,useEffect,useRef } from 'react';
import Spinner from "./Spinner";
import useFetch from './useFetch.js';
import Error from "./Error";
import NavBar from './NavBar';

import Default from './Default';
import Events from './Events';
function App() {
  return(
  <Router>
    <NavBar/>
  <Switch>
    <Route exact path="/">
      <Default/>
    </Route>
    <Route exact path="/Events">
      <Events/>
    </Route>
    <Route  path="*">
      <Error />
    </Route>
  </Switch>
</Router>
  );
}

export default App;
