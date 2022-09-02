using System;
using System.Collections.Generic;
using System.Text;

namespace RhubarbCloudClient
{
	public static class RhubarbLoadingFacts
	{
		public static string[] RhubarbFacts = new string[] {
			"Rhubarb has a web client",
			"Rhubarb is powered by RhuEngine",
			"Rhubarb has many rendering BackEnds",
			"Rhubarb has skin meshes",
			"Rhubarb supports EcmaScipt scripting",
			"Rhubarb doesn't like calling EcmaScipt JavaScript",
			"Rhubarb2 will never exists",
			"Rhubarb is a vegetable",
			".-. .... ..- -... .- .-. -...",
			"Rhubarb is open source",
			"<br /><br /><br /><br /><br />Fresh Rhubarb Pie<br />" +
			"- 1 ⅓ cups white sugar<br />"+
			"- 6 tablespoons all-purpose flour<br />"+
			"- 1 (14.1 ounce) package pastry for a double-crust 9-inch pie<br />" +
			"- 4 cups chopped rhubarb<br />"+
			"- 1 tablespoon butter"
		};

		public static string[] RandomFacts = new string[] {
			"Peanuts aren't nuts",
			"Eggplants are considered berries",
			"Pineapples are named after pinecones",
			"The largest recorded snowflake is 15 inches wide",
			"Sailors consider black cats good luck",
			"Alan Shepard played golf on the moon",
			"One species of ants can only be found in Manhattan, New York",
			"Around 30,000 rubber ducks were lost at sea in 1992",
			"Your teeth are unique",
			"Baby sea otters are unable to swim",
			"Snakes can predict earthquakes",
			"Cats can't taste sweet flavors",
			"If you close your eyes you can't see",
			"123456 is the most common password",
			"The chicken is the closest relative to the T-Rex",
			"Ravens are always aware when someone is watching them",
			"Baby spiders are called spiderlings",
			"The roar of a lion can be heard up to 5 miles away",
			"Flannel is mistaken as a synonym for plaid",
			"Faolan entire wardrobe is plaid",
			"Bananas are radioactive"
		};


		public static string[] FunnyFacts = new string[] {
			"Faolan likes trains",
			"RayTraxing was removed",
			"RayTraxing was added",
			"Pineapples",
			"<i class=\"bi bi-train-front\"></i>Trains",
			"Trees Grow",
			" <br />  <br /> You Could Make a Religion Out of This <br />   <br /> - No don't",
			"The index controller has buttons on it",
			"You should drink water",
			"Don't dig straight down",
			"KitKat was here<br /> adsewdaswxexxxxxxxxxxxx",
			"That this message intentionally left blank",
		};

		public static string[] GetSleepFacts = new string[] {
			"In a few hours the sun will rise",
			"Its dark outside",
			"Sleep is for the week",
			"Sleep is for the weak",
		};

		public static Dictionary<(int month, int day), string> DayFacts = new() {
			{
				(month:1,day:16),"Jan 29th is Appreciate a Dragon Day"
			},{
				(month:2,day:13),"Feb 13th is World Radio Day"
			},{
				(month:3,day:7),"Mar 7th is National Cereal Day"
			},{
				(month:3,day:14),Math.PI.ToString()
			},{
				(month:4,day:1),"Your shoes are untied"
			},{
				(month:5,day:29),"May 29th is Put a Pillow on Your Fridge Day"
			},{
				(month:6,day:19),"Jun 19th is National Watch Day"
			},{
				(month:7,day:10),"July 10th is National Kitten Day"
			},{
				(month:8,day:18),"Aug 18th is Never Give Up Day"
			},{
				(month:9,day:5),"Sept 5th is National Cheese Pizza Day"
			},{
				(month:9,day:19),"Sept 19th is Faolan's Birthday"
			},{
				(month:10,day:1),"Oct 1st is CD Player Day"
			},{
				(month:10,day:31),"Where in the bone zone"
			},{
				(month:11,day:12),"Nov 12th is National French Dip Day"
			},{
				(month:12,day:9),"Dec 9th is Lost and Found Day"
			}
		};

		public static string GetRandomFunnyFact(Random random) {
			return FunnyFacts[random.Next(FunnyFacts.Length - 1)];
		}

		public static string GetRandomFact(Random random) {
			return RandomFacts[random.Next(RandomFacts.Length - 1)];
		}

		public static string GetRandomRhubarbFact(Random random) {
			return RhubarbFacts[random.Next(RhubarbFacts.Length - 1)];
		}

		public static string GetRandomFactNoDate(Random random) {
			var ran = random.Next(9);
			if (ran <= 1) {
				return GetRandomFunnyFact(random);
			}
			if (ran <= 3) {
				return GetRandomFact(random);
			}
			return GetRandomRhubarbFact(random);
		}

		public static string GetRandomFact() {
			var currentTime = DateTime.Now;
			var randomGen = new Random();
			if (currentTime.Hour <= 5) {
				if (randomGen.Next(10) <= 1) {
					var num = randomGen.Next(GetSleepFacts.Length);
					return num == GetSleepFacts.Length ? $"It is {currentTime.Hour} in the morning" : GetSleepFacts[num];
				}
			}
			var random = GetRandomFactNoDate(randomGen);
			return DayFacts.TryGetValue((currentTime.Month, currentTime.Day), out var value) ? (randomGen.Next(10) <= 3) ? value : random : random;
		}
	}
}
