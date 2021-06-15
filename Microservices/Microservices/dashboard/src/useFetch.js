import { useState, useEffect } from "react";

const baseUrl = "http://localhost:52807/api/";

export default function useFetch(url) {
  const [data, setData] = useState(null);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function init() {
      try {
        const response = await fetch(baseUrl + url,{
          method:"GET",
        });
        if (response.ok) {
          const json = await response.json();
          setData(json);
        }else if(response.status==401){
        } 
        else {
          throw response;
        }
      } catch (e) {
        setError(e);
      } finally {
        setLoading(false);
      }
    }
    init(); 
  }, [url]);

  return { data, error, loading };
}
