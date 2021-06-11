import logo from './logo.svg';
import './App.css';
import { HubConnectionBuilder } from '@microsoft/signalr';
import React, { useState,useEffect,useRef } from 'react';



function App() {
  const [ connection, setConnection ] = useState(null);
  const [ chat, setChat ] = useState([]);
  const latestChat = useRef(null);  
  latestChat.current = chat;


  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
        .withUrl('http://localhost:53800/eventhub',{})
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
      <ul>
        {chat.map((el)=>{
          return <li>{el}</li>
        })}
        
      </ul>
    </div>
  );
}

export default App;