using Destec.CoreApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Destec.CoreApi.Controllers.Business
{
    [Route("api/[controller]")]
    public class AtividadeController : Controller
    {
        private readonly ApplicationDbContext db;

        public AtividadeController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult Execute([FromQuery] short code)
        {
            // ToDo: Get current activity

            // ToDo: If Exists, 
            if (true)
            {
                // If Has Interval Start
                if (true)
                {
                    // get difference from Now to StartInterval
                    // Add to Timespan 
                    // Clean StartInterval Date
                    // Return Current Activity Information plus this interval span;
                }
                else
                {
                    // set End Time
                    // Clean Current activity of user
                }
            }

            // ToDo: Get Next Acitivity available for user
            // Set User on Activity Group
            // Set Acitivty Id on User

            return Ok();
        }

        [HttpGet]
        public IActionResult Interval([FromQuery] short code)
        {
            // ToDo: Get current activity

            // ToDo: If Exists
            if (true)
            {
                // If Has Interval Start
                if (true)
                {
                    // get difference from Now to StartInterval
                    // Add to Timespan 
                    // Clean StartInterval Date
                    // Return Current Activity Information plus this interval span;
                }
                else
                {
                    // set Start Interval DateTime
                    // Return Ok to user
                }
            }
            else
            {
                // Return "No Activity" message
            }
            return Ok();
        }

        [HttpGet]
        public IActionResult Stop([FromQuery] short code)
        {
            // ToDo: Get current activity

            // ToDo: If Exists, 
            if (true)
            {
                // set End Time
                // Clean Current activity of user
            }
            else
            {
                // Return "No Activity" message
            }
            return Ok();
        }
    }
}
