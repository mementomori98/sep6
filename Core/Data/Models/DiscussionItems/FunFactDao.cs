using Core.Domain.DiscussionItems.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data.Models
{
    public class FunFactDao : DiscussionItemDao
    {
        internal static FunFactDao MapFunFactToDao(FunFactModel funFact)
        {
            return (FunFactDao)MapDiscussionItemToDao(
                funFact,
                new FunFactDao()
            );
        }

        internal FunFactModel MapToFunFact(long numberOfLikes, long numberOfDislikes, Interactions? interaction)
        {
            return (FunFactModel)MapToDiscussionItem(new FunFactModel(), numberOfLikes, numberOfDislikes, interaction);
        }
    }
}
