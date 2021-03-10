using ABC.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ABC.Shared.Interfaces
{
    public interface ISocialStatistics
    {
        SocialStatistics GetTwitterStatistics(DateTime requestTime);
    }
}
