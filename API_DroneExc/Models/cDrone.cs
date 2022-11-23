using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_DroneExc.Models
{
    public enum Model
    {
        Lightweight, Middleweight, Cruiserweight, Heavyweight
    }

    public enum State
    {
        IDLE, LOADING, LOADED, DELIVERING, DELIVERED, RETURNING
    }
    public class cDrone
    {
        public char[] serial_number = new char[100];
        public Model Model { get; set; }
        public float Weight_Limit { get; set; }
        public float Batt_cap { get; set; }
        public State State { get; set; }
        public List<cMedication> medication_List = new List<cMedication>();

        public cDrone()
        {
            
        }

        public char[] Serial_number
        {
            get => serial_number;

            set
            {
                serial_number = value;
            }
        }

    }
}