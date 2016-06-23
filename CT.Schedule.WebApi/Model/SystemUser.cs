using System;

namespace CT.Schedule.WebApi.Model
{
    public class SystemUser
    {

        public Guid UserID { get; set; }
        public string UserName { get; set; }

        public string Password { get; set; }

    }
}