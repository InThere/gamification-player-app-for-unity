using System;
namespace GamificationPlayer.Session
{
    [AttributeUsage(AttributeTargets.Field)]
    public class CampaignId : Attribute, IQueryable
    {
    }
}
