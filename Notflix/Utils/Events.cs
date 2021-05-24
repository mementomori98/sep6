using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;

namespace Notflix.Utils
{
    public static class Events
    {
        public static Action<KeyboardEventArgs> OnEnter(Action action)
        {
            return e =>
            {
                if (e.Key == "Enter")
                    action.Invoke();
            };
        }
    }
}