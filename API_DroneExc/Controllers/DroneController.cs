using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using API_DroneExc.Models;
using System.Threading.Tasks;

namespace API_DroneExc.Controllers
{
    public class DroneController : ApiController
    {
        static Dictionary<char[], cDrone> drones = new Dictionary<char[], cDrone>();
        
        // GET: Drone
        public IEnumerable<cDrone> Get_Drones()
        {
            return new List<cDrone>(drones.Values);
        }

        // GET: Drone/Details/sn
        public cDrone Get(char[] sn_Dr)
        {
            cDrone drn;
            drones.TryGetValue(sn_Dr, out drn);
            return drn;
        }  

        
        // POST: Drone/Create
        public bool Post([FromBody] cDrone drone)
        {
            cDrone drn;
            drones.TryGetValue(drone.serial_number, out drn);
            if (drn == null)
            {
                drones.Add(drn.serial_number, drone);
                return true;
            }
            else
                return false;

        }

        // POST: Drone/Edit/5
        public bool Load_Drone([FromBody]cDrone pDron ,List<cMedication> med_Items)
        {

            if (pDron.State == State.IDLE && pDron.Batt_cap > 25)
            {
                pDron.State = State.LOADING;
                for (int i = 0; i < med_Items.Count; i++)
                {
                    float limit = 0;
                    limit += med_Items[i].Weight;
                    while (limit <= pDron.Weight_Limit)
                    {
                        pDron.medication_List.Add(med_Items[i]);
                    }

                }

                return true;

            }
            else
                return false;

        }


        // GET: Drone/Medication_List/
        public List<cMedication> Check_Medications([FromBody]cDrone pDrone)
        {
            cDrone drn;
            cMedication medic;
            List<cMedication> medic_ret = new List<cMedication>();
            drones.TryGetValue(pDrone.Serial_number, out drn);

            if (pDrone.State == State.LOADED)
            {
                for (int i = 0; i < pDrone.medication_List.Count; i++)
                {
                    medic = pDrone.medication_List[i];
                    medic_ret.Add(medic);
                }               
            }

            return medic_ret;

        }

        //GET: Drone/Available
        public List<cDrone> Drone_Avlb()
        {
            List<cDrone> dr_avalbl = new List<cDrone>();
            List<cDrone> drones = (List<cDrone>) Get_Drones();
            for (int i=0; i < drones.Count; i++)
            {
               if( drones[i].State == State.IDLE || drones[i].State == State.DELIVERED)
                {
                    dr_avalbl.Add(drones[i]);
                }
            }
            return dr_avalbl;
        }

        //GET: Drone/Battery_Level 
        public float Batt_Level([FromBody] cDrone pDrone)
        {
            cDrone dr;
            drones.TryGetValue(pDrone.serial_number, out dr);

            return dr.Batt_cap;
            
        }


        // GET: Drone/Schedule_log



    }
}
