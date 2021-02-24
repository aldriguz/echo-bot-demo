using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoBotDemo.Models
{
    public class ConversationData
    {
        // Track whether we have already asked for user's name
        public bool PromptedUserForName { get; set; } = false;
    }
}
