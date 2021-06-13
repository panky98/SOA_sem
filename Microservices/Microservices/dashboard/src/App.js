import logo from './logo.svg';
import './App.css';
import { HubConnectionBuilder } from '@microsoft/signalr';
import React, { useState,useEffect,useRef } from 'react';
import Spinner from "./Spinner";
import useFetch from './useFetch.js';

function App() {
  const {data:data1c, loading1c, error1c} = useFetch("getAllSensorData/1c-bf-ce-15-ec-4d");
  const {data:data00, loading00, error00} = useFetch("getAllSensorData/00-0f-00-70-91-0a");
  //const {data:datab8, loadingb8, errorb8} = useFetch("getAllSensorData/b8-27-eb-bf-9d-51");

  const [ connection, setConnection ] = useState(null);
  const [ chat, setChat ] = useState([]);
  const latestChat = useRef(null);  
  latestChat.current = chat;


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
if(error1c) throw error1c;
if(error00) throw error00;
if(loading1c||loading00) return <Spinner/>
  return (
    <div>
      <ul>
        {chat.map((el)=>{
          return <li>{el}</li>
        })}
        
      </ul>
    </div>
  );
}

export default App;
