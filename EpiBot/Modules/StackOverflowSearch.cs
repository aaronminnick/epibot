using Discord.Interactions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Threading.Tasks;


namespace EpiBot.Modules
{
  public class StackOverflowSearch : InteractionModuleBase<SocketInteractionContext>
  {
    [SlashCommand("stackoverflow", "search for a question on stack overflow")]
    public async Task Search(string question, string sort = "activity")
    {
      RestClient client = new RestClient("https://api.stackexchange.com/2.3/");
      RestRequest request = new RestRequest($"search?page=1&pagesize=5&order=desc&sort={sort}&intitle={question}&site=stackoverflow", Method.Get);
      RestResponse response = await client.ExecuteGetAsync(request);
      await RespondAsync(
        text: $"Here are some stackoverflow results similar to \"{question}\"",
        embed: ParseToEmbed(response.Content));
    }

    private Discord.Embed ParseToEmbed(string content)
    {
      JObject parsed = JsonConvert.DeserializeObject<JObject>(content);
      Console.WriteLine(parsed.ToString());
      var embed = new Discord.EmbedBuilder();
      if (parsed["items"].HasValues)
      {
        foreach(var item in parsed["items"])
        {
          embed.AddField("test", $"[{item["title"]}]({item["link"]})");
        }
      }
      else
      {
        embed.AddField("test", "no results");
      }
      return embed.Build();
    }
  }
}