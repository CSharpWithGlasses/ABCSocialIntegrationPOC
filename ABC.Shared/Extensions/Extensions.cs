using System;
using System.Collections.Generic;
using System.Text;

namespace ABC.Shared.Extensions
{
    public static class Extensions
    {
        public static T ExecuteWithNoFailExceptions<T>(this Func<T> taskAction, List<string> errorMessages, out bool result)
        {
            
            try
            {
                var data = taskAction();
                result = true;
                return data;
            }
            catch(Exception ex)
            {
                result = false;
                errorMessages.Add(ex.ToString());
                return default(T);
            }
        }

        public static bool ExecuteWithNoFailExceptions<T>(this Action taskAction, List<string> errorMessages)
        {

            try
            {
                taskAction();
                return true;
            }
            catch (Exception ex)
            {
                errorMessages.Add(ex.ToString());
                return false;
            }
        }
    }
}
