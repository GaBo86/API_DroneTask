using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using API_DroneExc.Models;
using Microsoft.Win32.TaskScheduler;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace API_DroneExc.Controllers
{
    public class DroneController : ApiController
    {

        // GET: Drone
        public cDrone Drones()
        {
            StreamReader file = File.OpenText(@"C:\Users\GaBo\source\repos\API_DroneExcersice\API_DroneExc\API_DroneExc\");
            string json = file.ReadToEnd();
            cDrone drone = JsonSerializer.Deserialize<cDrone>(json);
            
            return drone;
        }

        // GET: Drone/Details/sn
        public cDrone Drone(char[] sn_Dr)
        {
            StreamReader file = File.OpenText(@"C:\Users\GaBo\source\repos\API_DroneExcersice\API_DroneExc\API_DroneExc\");
            string json = file.ReadToEnd();
            cDrone dr = new cDrone();

            dynamic myArray = JsonSerializer.Deserialize(json);

            foreach(var item in myArray)
            {
                if (item.serial_number == sn_Dr)
                    dr = item;
               

            }
            return dr;
           
        }


        // POST: Drone/Create
        public bool Post(char[] pSerial, Model pModel, float pWeight, float pBattery, State pState, List<cMedication> pL_Medication)
        {
            cDrone drn = new cDrone();
            drn.serial_number = pSerial;
            drn.Model = pModel;
            drn.Weight_Limit = pWeight;
            drn.Batt_cap = pBattery;
            drn.State = pState;
            drn.medication_List = pL_Medication;

            var options = new JsonSerializerOptions { WriteIndented = true };
            JsonSerializer.Serialize(drn, options);

            return true;

        }

        // POST: Drone/Edit/5
        public bool Load_Drone(char[] sn_drone, List<cMedication> med_Items)
        {
            StreamReader file = File.OpenText(@"C:\Users\GaBo\source\repos\API_DroneExcersice\API_DroneExc\API_DroneExc\");
            string json = file.ReadToEnd();
            cDrone dr = new cDrone();

            dynamic myArray = JsonSerializer.Deserialize(json);

            foreach (var item in myArray)
            {
                if (item.serial_number == sn_drone)
                    dr = item;
            }
            
            if (dr.State == State.IDLE && dr.Batt_cap > 25)
            {
                dr.State = State.LOADING;
                for (int i = 0; i < med_Items.Count; i++)
                {
                    float limit = 0;
                    limit += med_Items[i].Weight;
                    while (limit <= dr.Weight_Limit)
                    {
                        dr.medication_List.Add(med_Items[i]);
                    }

                }

                return true;

            }
            else
                return false;

        }


        // GET: Drone/Medication_List/
        public List<cMedication> Check_Medications(char[] sn_Dr)
        {
            StreamReader file = File.OpenText(@"C:\Users\GaBo\source\repos\API_DroneExcersice\API_DroneExc\API_DroneExc\");
            string json = file.ReadToEnd();
            cDrone dr = new cDrone();

            dynamic myArray = JsonSerializer.Deserialize(json);

            foreach (var item in myArray)
            {
                if (item.serial_number == sn_Dr)
                    dr = item;
            }

            cMedication medic = new cMedication();
            List<cMedication> medic_ret = new List<cMedication>();

            if (dr.State == State.LOADED)
            {
                for (int i = 0; i < dr.medication_List.Count; i++)
                {
                    medic = dr.medication_List[i];
                    medic_ret.Add(medic);
                }
            }

            return medic_ret;

        }

        //GET: Drone/Available
        public List<cDrone> Drone_Avlb()
        {
            StreamReader file = File.OpenText(@"C:\Users\GaBo\source\repos\API_DroneExcersice\API_DroneExc\API_DroneExc\");
            string json = file.ReadToEnd();

            cDrone dr = JsonSerializer.Deserialize<cDrone>(json);

            List<cDrone> dr_avalbl = new List<cDrone>();
            for (int i = 0; i < dr.medication_List.Count; i++)
            {
                if (dr.State == State.IDLE || dr.State == State.DELIVERED)
                {
                    dr_avalbl.Add(dr);
                }
            }
            return dr_avalbl;
        }

        //GET: Drone/Battery_Level 
        public float Batt_Level(char[] pSn_dr)
        {
            StreamReader file = File.OpenText(@"C:\Users\GaBo\source\repos\API_DroneExcersice\API_DroneExc\API_DroneExc\");
            string json = file.ReadToEnd();
            cDrone dr = new cDrone();

            dynamic myArray = JsonSerializer.Deserialize(json);

            foreach (var item in myArray)
            {
                if (item.serial_number == pSn_dr)
                    dr = item;
            }

            return dr.Batt_cap;

        }


        // GET: Drone/Schedule_log
        public void BattLog_Sched ()
        {
            StreamReader file = File.OpenText("jDrone.json");
            string json = file.ReadToEnd();
            

            List<float> Bat_Level = new List<float>();

            dynamic myArray = JsonSerializer.Deserialize(json);

            foreach (var item in myArray)
            {
                Bat_Level.Add(item.battery_capacity);
            }

            string Batt_File = "Batt_Check_level.json";
            string json_List = JsonSerializer.Serialize(Bat_Level);
            File.WriteAllText(Batt_File,json_List);

            string CheckLvlBatt_Task = "Check Drones Battery Level";
            TaskDefinition TD = TaskService.Instance.NewTask();

            TD.RegistrationInfo.Author = "GaBoCo";
            TD.RegistrationInfo.Description = "Checking the battery levels of the drones";
            TD.Actions.Add(new ExecAction(@"Batt_File"));

            TaskService.Instance.RootFolder.RegisterTaskDefinition(CheckLvlBatt_Task, TD);



           


        }


    }

}
