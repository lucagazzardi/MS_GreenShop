using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace User.Data
{
    public class UserContextSeed
    {
        public void Seed(UserContext context)
        {
            context.Database.Migrate();
        }
    }
}