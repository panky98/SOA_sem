import logo from './logo.svg';
import './App.css';
import { HubConnectionBuilder } from '@microsoft/signalr';
import React, { useState,useEffect,useRef } from 'react';
import Spinner from "./Spinner";
import useFetch from './useFetch.js';

function Events(){
const [data1c, setData1c] = useState(null);
const [error1c, setError1c] = useState(false);
const [loading1c, setLoading1c] = useState(true);
//const {data:data00, loading00, error00} = useFetch("getAllSensorData/00-0f-00-70-91-0a");
const[buttonData,setButtonData]=useState();
const[showSpinner,setShowSpinner]=useState(true);
//const[test,setTest]=useState(fetch("http://localhost:52807/api/getAllSensorData/1c-bf-ce-15-ec-4d",{
//  method:"GET",  }));
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


    fetch("http://localhost:52807/api/getAllSensorData/1c-bf-ce-15-ec-4d",{
          method:"GET",
        }).then(p=>{
            if(p.ok){
                p.json().then(data=>{
                    setData1c(data);
                    setShowSpinner(false);
                })
            }
        }).catch(e=>{console.log(e)})
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
  fetch("http://localhost:52807/api/getAllSensorData/1c-bf-ce-15-ec-4d",{
    "method":"GET"
  }).then(p=>{
    if(p.ok){
      p.json().then(data=>{
          console.log(data);
        setButtonData(data);
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
      <tbody>
          <td>{data1c.co}</td>
          <td>{data1c.humidity}</td>
          <td>{data1c.light}</td>
          <td>{data1c.motion}</td>
          <td>{data1c.smoke}</td>
          <td>{data1c.temp}</td>
      </tbody>
    </table>
  </div>
  
);
}
export default Events;