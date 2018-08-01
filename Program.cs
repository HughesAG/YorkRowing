using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataFeedRowing
{
    public class RiverState { 

        public float currentWindspeed;
        public float currentWindgust;
        public float currentRiverstage;
        public float forecastWindspeed;
        public float forecastWindgust;
        public float forecastRainfallLocal;
        public float forecastRainfallUpstream;


        public RiverState()
        {
            currentWindspeed = 0;
            currentWindgust = 0;
            currentRiverstage = 0;
            forecastWindspeed = 0;
            forecastWindgust = 0;
            forecastRainfallLocal = 0;
            forecastRainfallUpstream = 0;
        }
        public string InitializeShoothillAPI()
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                string uri = "http://riverlevelsapi.shoothill.com/ApiAccount/ApiLogin";
                string data = "{'PublicApiKey':'4a361716-61ef-4f42-9768-9393cf68c96d', 'ApiVersion':'2'}";
                string result = wc.UploadString(uri, data);
                dynamic authorizedSession = JsonConvert.DeserializeObject<dynamic>(result);
                Console.WriteLine(authorizedSession);
                Console.WriteLine(" Result is: " + result);
                return authorizedSession.SessionHeaderId;
            }

        }
        public string RowingFlagStateCurrent(String crewType)
        {
            string flagState = "Green";
            if(currentWindspeed > 13) {
                flagState = "Amber";
                }
            if (crewType == "Senior" && currentWindgust > 18 && currentWindgust < 22)
            {
                flagState = "Red / Amber";
            }
            if (crewType == "Novice" && currentWindgust > 15 && currentWindgust < 18)
            {
                flagState = "Red / Amber";
            }
            if (crewType == "Senior" && currentWindgust >= 22 )
            {
                flagState = "Red";
            }
            if (crewType == "Novice" && currentWindgust >= 18 )
            {
                flagState = "Red";
            }
            return flagState;
        }
        public string RowingFlagStateFuture(String crewType)
        {
            string flagState = "Green";
            if (currentWindspeed > 13)
            {
                flagState = "Amber";
            }
            if (crewType == "Senior" && forecastWindgust > 18 && forecastWindgust < 22)
            {
                flagState = "Red / Amber";
            }
            if (crewType == "Novice" && forecastWindgust > 15 && forecastWindgust < 18)
            {
                flagState = "Red / Amber";
            }
            if (crewType == "Senior" && forecastWindgust >= 22)
            {
                flagState = "Red";
            }
            if (crewType == "Novice" && forecastWindgust >= 18)
            {
                flagState = "Red";
            }
            return flagState;
        }
        public string RowingDecisionCurrent()
        {
            string outingState = "No outings as default";
            if (currentRiverstage < 1.5){
                outingState = "All crews can row anything";
            }else if (currentRiverstage >= 1.5 && currentRiverstage < 1.8){
                outingState = "no novice 4s - all with safety launch";
            }else if(currentRiverstage >= 1.8 && currentRiverstage < 2.2){
                outingState = "no novices at all - seniors with safety launch";
            }else if(currentRiverstage >= 2.2){
                outingState = "No outings for all crews";
            }


            return outingState;
        }

    }

class Program
    {

        static void Main(string[] args)
        {
            // Instantiate class
            RiverState test = new RiverState();
            String directoryOutput = "C:/Projects/Activities/York ULP/DataFeedRowing/test.txt";
            // String directoryOutput = "test.txt";
            // Setup Shoothill API
            Console.WriteLine("Getting data from Shoothill API"+"\n");
            string SessionHeaderId = string.Empty;
            SessionHeaderId = test.InitializeShoothillAPI();
            Console.WriteLine("SessionHeaderId is: " + SessionHeaderId + "\n");
            CurrentState RiverCurrent = new CurrentState(SessionHeaderId, directoryOutput);
            System.IO.File.AppendAllText(directoryOutput, "CS - SessionHeaderId is: " + RiverCurrent.SessionHeaderId + "\n");
            RiverCurrent.ReturnData();
            
            test.currentWindspeed = RiverCurrent.ReturnWindspeed();
            System.IO.File.AppendAllText(directoryOutput, "\n" + "Current Windspeed (In main) is: " + test.currentWindspeed + "\n");
            test.currentWindgust = RiverCurrent.ReturnWindgust();
            System.IO.File.AppendAllText(directoryOutput, "\n" + "Current Windgust (In main) is: " + test.currentWindgust + "\n");
            test.currentRiverstage = RiverCurrent.ReturnRiverStage();
            System.IO.File.AppendAllText(directoryOutput, "\n" + "Current Riverstage (In main) is: " + test.currentRiverstage + "\n");

            FutureState WeatherFuture = new FutureState(SessionHeaderId, directoryOutput);
            System.IO.File.AppendAllText(directoryOutput, "\n" + "FS - SessionHeaderId is: " + WeatherFuture.SessionHeaderId + "\n");
            WeatherFuture.ReturnData();
            test.forecastWindspeed = WeatherFuture.ReturnWindspeed();
            System.IO.File.AppendAllText(directoryOutput, "\n" + "Forecast Windspeed (In main) is: " + test.forecastWindspeed + "\n");
            test.forecastWindgust = WeatherFuture.ReturnWindgust();
            System.IO.File.AppendAllText(directoryOutput, "\n" + "Forecast Windgust(In main) is: " + test.forecastWindgust + "\n");
            test.forecastRainfallLocal = WeatherFuture.ReturnPrecipitationLocal();
            System.IO.File.AppendAllText(directoryOutput, "\n" + "Forecast Chance of ppt Topcliffe (In main) is:" + test.forecastRainfallLocal + "\n");
            
            System.IO.File.AppendAllText(directoryOutput, "\n" + "Current Flag state for Novice crew is: " + test.RowingFlagStateCurrent("Novice") + "\n");
            Console.WriteLine("Current Flag state for Novice crew is: " + test.RowingFlagStateCurrent("Novice") + "\n");
            System.IO.File.AppendAllText(directoryOutput, "\n" + "Current Flag state for Senior crew is: " + test.RowingFlagStateCurrent("Senior") + "\n");
            Console.WriteLine("Current Flag state for Senior crew is: " + test.RowingFlagStateCurrent("Senior") + "\n");
            System.IO.File.AppendAllText(directoryOutput, "\n" + "Forecast Flag state for Novice crew is: " + test.RowingFlagStateFuture("Novice") + "\n");
            Console.WriteLine("Forecast Flag state for Novice crew is: " + test.RowingFlagStateFuture("Novice") + "\n");
            System.IO.File.AppendAllText(directoryOutput, "\n" + "Forecast Flag state for Senior crew is: " + test.RowingFlagStateFuture("Senior") + "\n");
            Console.WriteLine("Forecast Flag state for Senior crew is: " + test.RowingFlagStateFuture("Senior") + "\n");
            System.IO.File.AppendAllText(directoryOutput, "\n" + "Current river stage state for all crews is: " + test.RowingDecisionCurrent() + "\n");
            Console.WriteLine("Current river stage state for all crews is: " + test.RowingDecisionCurrent() + "\n");
            Console.WriteLine("Forecast local rain is " + test.forecastRainfallLocal + " as a percentage" + "\n");

            // Keep console window open in debug mode
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

        }


    }
}
