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

const[averageCo,setAverageCo]=useState(0);
const[maximumCo,setMaximumCo]=useState(0);
const[minimumCo,setMinimumCo]=useState(0);

const[averageHumidity,setAverageHumidity]=useState(0);
const[maximumHumidity,setMaximumHumidity]=useState(0);
const[minimumHumidity,setMinimumHumidity]=useState(0);

const[averageLpg,setAverageLpg]=useState(0);
const[maximumLpg,setMaximumLpg]=useState(0);
const[minimumLpg,setMinimumLpg]=useState(0);

const[averageSmoke,setAverageSmoke]=useState(0);
const[maximumSmoke,setMaximumSmoke]=useState(0);
const[minimumSmoke,setMinimumSmoke]=useState(0);

const[averageTemp,setAverageTemp]=useState(0);
const[maximumTemp,setMaximumTemp]=useState(0);
const[minimumTemp,setMinimumTemp]=useState(0);

const[averageMS,setAverageMS]=useState(0);
const[maximumMS,setMaximumMS]=useState(0);
const[minimumMS,setMinimumMS]=useState(0);

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


        let avgCo=0;
        let maxCo=0;
        let minCo=1;

        let avgHumidity=0;
        let minHumidity=300;
        let maxHumidity=0;

        let avgLpg=0;
        let maxLpg=0;
        let minLpg=1; 

        let avgSmoke=0;
        let maxSmoke=0;
        let minSmoke=1;

        let avgTemp=0;
        let maxTemp=0;
        let minTemp=1000;

        let avgMS=0;
        let maxMS=0;
        let minMS=999999999999999;

        
        for (let i = 0; i < data.length; i++) {
          const item = data[i];

          avgCo+=parseFloat(item.co);
          if(parseFloat(item.co)>parseFloat(maxCo))
              maxCo=parseFloat(item.co);
          if(parseFloat(item.co)<parseFloat(minCo))
              minCo=parseFloat(item.co);
          
          avgHumidity+=parseFloat(item.humidity);
          if(parseFloat(item.humidity)>parseFloat(maxHumidity))
            maxHumidity=parseFloat(item.humidity);
          if(parseFloat(item.humidity)<parseFloat(minHumidity))
            minHumidity=parseFloat(item.humidity);

          avgLpg+=parseFloat(item.lpg);
          if(parseFloat(item.lpg)>parseFloat(maxLpg))
              maxLpg=parseFloat(item.lpg);
          if(parseFloat(item.lpg)<parseFloat(minLpg))
              minLpg=parseFloat(item.lpg); 
              
          avgSmoke+=parseFloat(item.smoke);
          if(parseFloat(item.smoke)>parseFloat(maxSmoke))
              maxSmoke=parseFloat(item.smoke);
          if(parseFloat(item.smoke)<parseFloat(minSmoke))
              minSmoke=parseFloat(item.smoke);
              
          avgTemp+=parseFloat(item.temp);
          if(parseFloat(item.temp)>parseFloat(maxTemp))
              maxTemp=parseFloat(item.temp);
          if(parseFloat(item.Temp)<parseFloat(minTemp))
              minTemp=parseFloat(item.temp);   
              
          avgMS+=parseFloat(item.ms);
          if(parseFloat(item.ms)>parseFloat(maxMS))
              maxMS=parseFloat(item.ms);
          if(parseFloat(item.MS)<parseFloat(minMS))
              minMS=parseFloat(item.ms); 

          content.push(<tr key={i}><td>{item.co}</td><td>{item.humidity}</td><td>{item.light}</td><td>{item.lpg}</td><td>{item.motion}</td><td>{item.smoke}</td><td>{item.temp}</td><td>{item.ms}</td></tr>);
        }
        avgCo/=parseFloat(data.length);
        console.log(avgCo);
        console.log(maxCo);
        console.log(minCo);
        avgHumidity/=parseFloat(data.length);
        avgLpg/=parseFloat(data.length);
        avgSmoke/=parseFloat(data.length);
        avgTemp/=parseFloat(data.length);
        avgMS/=parseFloat(data.length);

        setAverageCo(parseFloat(avgCo));
        setMaximumCo(parseFloat(maxCo));
        setMinimumCo(parseFloat(minCo));

        setAverageHumidity(parseFloat(avgHumidity));
        setMaximumHumidity(parseFloat(maxHumidity));
        setMinimumHumidity(parseFloat(minHumidity));

        setAverageLpg(parseFloat(avgLpg));
        setMaximumLpg(parseFloat(maxLpg));
        setMinimumLpg(parseFloat(minLpg));

        setAverageSmoke(parseFloat(avgSmoke));
        setMaximumSmoke(parseFloat(maxSmoke));
        setMinimumSmoke(parseFloat(minSmoke));

        setAverageTemp(parseFloat(avgTemp));
        setMaximumTemp(parseFloat(maxTemp));
        setMinimumTemp(parseFloat(minTemp));

        setAverageMS(parseFloat(avgMS));
        setMaximumMS(parseFloat(maxMS));
        setMinimumMS(parseFloat(minMS));

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
    <button onClick={()=>GetData()}> Get Sensor Data</button>
    {buttonClicked&&<div>
    <p>Co: max:{maximumCo}   min:{minimumCo}  avg:{averageCo}</p>
    <p>Humidity: max:{maximumHumidity}   min:{minimumHumidity}  avg:{averageHumidity}</p>
    <p>Lpg: max:{maximumLpg}   min:{minimumLpg}  avg:{averageLpg}</p>
    <p>Smoke: max:{maximumSmoke}   min:{minimumSmoke}  avg:{averageSmoke}</p>
    <p>Temp: max:{maximumTemp}   min:{minimumTemp}  avg:{averageTemp}</p>
    <p>MS: max:{maximumMS}   min:{minimumMS}  avg:{averageMS}</p>
    </div>}
    <table class="table table-dark table-striped">
      <thead>
        <tr>
          <td>Co</td>
          <td>Humidity</td>
          <td>Light</td>
          <td>Lpg</td>
          <td>Motion</td>
          <td>Smoke</td>
          <td>Temp</td>
          <td>MS</td>
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