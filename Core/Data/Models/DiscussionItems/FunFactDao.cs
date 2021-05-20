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
        internal static FunFactDao MapFunFactToDao(FunFact funFact)
        {
            return (FunFactDao)MapDiscussionItemToDao(
                funFact,
                new FunFactDao()
            );
        }

        internal FunFact MapToFunFact(long numberOfLikes, long numberOfDislikes, UserDiscussionItemInteractionType? interaction)
        {
            return (FunFact)MapToDiscussionItem(new FunFact(), numberOfLikes, numberOfDislikes, interaction);
        }
    }
}
