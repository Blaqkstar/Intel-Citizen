using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC_Player_Intel_App
{
    public class Player
    {
        private string name;
        private string enlistedDate;
        private string url;
        private string orgName;
        private string orgURL;
        private string bio;
        private int bounty;
        private string notes;
        private bool isOnTargetList;
        private bool isOnWhiteList;


        public string Name
        {
            set { name = value; }
            get { return name; }
        }

        public string EnlistedDate
        {
            set { enlistedDate = value; }
            get { return enlistedDate; }
        }
        public string URL
        {
            set { url = value; }
            get { return url; }
        }

        public string OrgName
        {
            set { orgName = value; }
            get { return orgName; }
        }

        public string OrgURL
        {
            set { orgURL = value; }
            get { return orgURL; }
        }
        public string Bio
        {
            set { bio = value; }
            get { return bio; }
        }

        public int Bounty
        {
            set { bounty = value; }
            get { return bounty; }
        }

        public string Notes
        {
            set { notes = value; }
            get { return notes; }
        }
        public bool IsOnTargetList
        {
            set { isOnTargetList = value; }
            get { return isOnTargetList; }
        }
        public bool IsOnWhiteList
        {
            set { isOnWhiteList = value; }
            get { return isOnWhiteList; }
        }
    }
}
