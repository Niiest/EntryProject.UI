﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EntryProject.UI.ViewModels
{
    public class SimpleResponse
    {
        public bool HasErrors { get; set; }
        public List<string> Messages { get; set; }

        public SimpleResponse AddErrorMessage(string error)
        {
            HasErrors = true;
            if (Messages == null)
            {
                Messages = new List<string> { error };
            }
            else
            {
                Messages.Add(error);
            }

            return this;
        }
    }
}
