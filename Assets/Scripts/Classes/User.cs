using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleJSON;

namespace Assets.Scripts.Classes
{

    public class User
    {
        public static User Parse(JSONNode json)
        {
            User user = new User { ID = Convert.ToInt16(json["user"]["id"].Value), Name = json["user"]["name"].Value };

            return user;
        }

        public int ID { get; set; }
        public string Name { get; set; }
    }
}
