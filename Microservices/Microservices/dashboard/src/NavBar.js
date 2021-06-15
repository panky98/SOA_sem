import React from 'react'

import { Link, NavLink } from "react-router-dom";
const activeStyle = {
  color: "purple",
};
function NavBar() {
    return (

      <header>
      <nav >
        <ul >
          <li>
            <Link activeStyle={activeStyle} to="/">
                Sensor Data
            </Link>
          </li>    
          <li>
          <Link activeStyle={activeStyle} to="/Events">
                Sensor Events
          </Link>           
          </li>  
        </ul>
      </nav>
      </header>
    )
}

export default NavBar
