using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLAB.Bot.Modules
{
    [Group("test")]
    [Summary("Test module")]
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        [Summary("Echoes back what you said")]
        [Alias("repeat", "say")]
        public async Task EchoAsync(
            [Summary("What you want the bot to say")]object input,
            [Summary("How many times you want to repeat the thing")]int amount = 1,
            [Summary("True if you want spaces between repeats")]bool separated = true)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < amount; i++)
            {
                builder.Append(input);
                if (separated)
                    builder.Append(' ');
            }

            foreach (string msg in Utilities.Split(builder, 2000))
            {
                await ReplyAsync(msg);
            }
        }
    }
}
