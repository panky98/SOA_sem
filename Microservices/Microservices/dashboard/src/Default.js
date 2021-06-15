import logo from './logo.svg';
import './App.css';
import { HubConnectionBuilder } from '@microsoft/signalr';
import React, { useState,useEffect,useRef } from 'react';
import Spinner from "./Spinner";
import useFetch from './useFetch.js';


function Default(){


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
function GetData()
{
  fetch("http://localhost:52807/api/getAllSensorData/"+sensorRef.current,{
    "method":"GET"
  }).then(p=>{
    if(p.ok){
      p.json().then(data=>{
          console.log(data);
        setButtonData(data);
        setButtonClicked(true);
        let content = [];
        for (let i = 0; i < data.length; i++) {
          const item = data[i];
          content.push(<tr key={i}><td>{item.co}</td><td>{item.humidity}</td><td>{item.light}</td><td>{item.motion}</td><td>{item.smoke}</td><td>{item.temp}</td></tr>);
        }
       setTableContent(content);
      });
    }});
}
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
    <div>
    {showSpinner && <Spinner/> }
    </div>
    <div>
      <select onChange={(ev)=>{sensorRef.current=ev.currentTarget.value}}>
                                    <option value="1c-bf-ce-15-ec-4d">1c-bf-ce-15-ec-4d</option>
                                    <option value="00-0f-00-70-91-0a">00-0f-00-70-91-0a</option>
                                    <option value="b8-27-eb-bf-9d-51">b8-27-eb-bf-9d-51</option>
                                    </select>
                                </div>
    <button onClick={()=>GetData()}> Test</button>
    <table>
      <thead>
        <tr>
          <td>Co</td>
          <td>Humidity</td>
          <td>Light</td>
          <td>Motion</td>
          <td>Smoke</td>
          <td>Temp</td>
        </tr>
      </thead>
      {buttonClicked&&<tbody>
          {tableContent}
      </tbody>}
    </table>
  </div>
  
);
}
export default Default;