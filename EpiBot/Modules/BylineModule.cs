using Discord.Interactions;
using EpiBot.Models;
using FuzzySharp;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EpiBot.Modules
{
  public class BylineModule : InteractionModuleBase<SocketInteractionContext>
  {
    private readonly EpiBotContext _db;

    public BylineModule(IServiceProvider services)
    {
      _db = services.GetRequiredService<EpiBotContext>();
    }

    [SlashCommand("byline-register", "register new byline for user")]
    public async Task BylineRegister(
      [Summary("Name for byline that will appear in commits")]
      string name,
      [Summary("github email address")]
      string email)
    {
      _db.Bylines.Add(new Byline { Name = name, Email = email });
      var result = _db.SaveChanges();
      if (result == 1)
      {
        await RespondAsync("Byline added");
      }
      else
      {
        await RespondAsync("Unable to add byline");
      }
    }

    [SlashCommand("byline-generate", "create byline with provided names")]
    public async Task BylineGenerate(
      [Summary("First byline name")]
      string name1,
      [Summary("Second byline name")]
      string name2 = "",
      [Summary("Third byline name")]
      string name3 = "",
      [Summary("Fourth byline name")]
      string name4 = "")
    {
      List<Byline> bylines = new List<Byline>();
      bylines.Add(FindClosestByline(name1));
      if (name2 != "")
      {
        bylines.Add(FindClosestByline(name2));
      }
      if (name3 != "")
      {
        bylines.Add(FindClosestByline(name3));
      }
      if (name4 != "")
      {
        bylines.Add(FindClosestByline(name4));
      }
      string response = "";
      bool foundAll = true;
      foreach (Byline byline in bylines)
      {
        if (byline != null)
        {
          response += byline.ToString();
          response += "\n";
        }
        else
        {
          foundAll = false;
        }
      }
      if (foundAll)
      {
        await RespondAsync(response);
      }
      else
      {
        await RespondAsync("One or more bylines not found: please check name spellings");
      }
    }

    private Byline FindClosestByline(string nameToFind)
    {
      int bestRatio = -1;
      Byline bestByline = null;
      foreach (Byline byline in _db.Bylines)
      {
        int thisRatio = Fuzz.Ratio(nameToFind, byline.Name);
        if (thisRatio > bestRatio)
        {
          bestRatio = thisRatio;
          bestByline = byline;
        }
      }
      return bestByline;
    }
  }
}