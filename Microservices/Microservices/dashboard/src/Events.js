import logo from './logo.svg';
import './App.css';
import { HubConnectionBuilder } from '@microsoft/signalr';
import React, { useState,useEffect,useRef } from 'react';
import Spinner from "./Spinner";
import useFetch from './useFetch.js';


function Events(){


const[buttonData,setButtonData]=useState();
const[tableContent,setTableContent]=useState([]);
const[buttonClicked,setButtonClicked]=useState(false);
const sensorRef=useRef();
const[showSpinner,setShowSpinner]=useState(true);

const [ connection, setConnection ] = useState(null);
const [ chat, setChat ] = useState([]);
const latestChat = useRef(null);  
latestChat.current = chat;
sensorRef.current="1c-bf-ce-15-ec-4d";

useEffect(() => {
  const newConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:52800/eventhub',{})
      .withAutomaticReconnect()
      .build();

  setConnection(newConnection);

}, []);


useEffect(() => {
if (connection) {
    connection.start()
        .then(async (result) => {
            console.log('Connected!');
            connection.on('ReceiveMessage', message => {
              console.log("PRIMLJENO: "+message);
              const updatedChat = [...latestChat.current];
              updatedChat.push(message);
              setChat(updatedChat);
          });
        await connection.send("AddToGroup","EventGroup").then(result=>{
            console.log("RESULT "+result);
        }).catch(excp=>{
            console.log("Exception: "+excp);
        })
          })
          .catch(e => console.log('Connection failed: ', e));
      }
}, [connection]);
useEffect(()=>{
return async function cleanup() {
    if(connection){
        await connection.send("RemoveFromGroup","EventGroup").then(result=>{
            console.log("RESULT "+result);
        }).catch(excp=>{
            console.log("Exception: "+excp);
        })
    }
  };    
},[connection]);
return (
  <div>
    {chat.map((el)=>{
          return <li>{el}</li>
        })}
  </div>
  
);
}
export default Events;