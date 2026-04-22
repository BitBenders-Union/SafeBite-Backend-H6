using System;
using System.Collections.Generic;

namespace SafeBiteApi.Utilities
{
    public static class AllergyMappings
    {
        public static readonly Dictionary<string, string[]> Mappings =
            new(StringComparer.OrdinalIgnoreCase)
            {
                // PORK / GRIS
                {
                    "gris", new[]
                    {
                        "gris", "svinekød", "svinekod", "svin", "pork", "porc", "schweinefleisch",
                        "ham", "skinke", "schinken", "bacon", "speck", "prosciutto", "serrano",
                        "parmaskinke", "parma ham", "salami", "pølse", "sausage", "bratwurst",
                        "wiener", "frankfurter", "spegepølse", "lardo", "pancetta"
                    }
                },

                // BEEF / OKSEKØD
                {
                    "oksekød", new[]
                    {
                        "oksekød", "okse", "ko", "rind", "rindfleisch", "beef", "boeuf",
                        "hakket oksekød", "hakket okse", "ground beef", "minced beef",
                        "oksesteg", "rostbeef", "roastbeef", "oksekraft", "beef stock",
                        "beef broth", "oksefond", "oksebouillon"
                    }
                },

                // CHICKEN / KYLLING
                {
                    "kylling", new[]
                    {
                        "kylling", "høne", "høns", "chicken", "poulet", "huhn", "hähnchen",
                        "kalkun", "turkey", "truthahn", "chicken broth", "chicken stock",
                        "kyllingefond", "kyllingebouillon", "chicken fat", "kyllingefedt"
                    }
                },

                // MILK / LACTOSE
                {
                    "lactose", new[]
                    {
                        "lactose", "laktose", "laktos", "laktosa", "milk", "mælk", "melk", "mjölk",
                        "milch", "vollmilch", "whole milk", "hel mælk", "skimmed milk", "skummetmælk",
                        "milk powder", "mælkepulver", "milk protein", "whey", "valle", "molke",
                        "casein", "kasein", "caseinate", "cream", "fløde", "sahne", "grädde",
                        "butter", "smør", "cheese", "ost", "käse", "yoghurt", "yogurt",
                        "buttermilk", "kærnemælk", "condensed milk"
                    }
                },
                {
                    "mælk", new[]
                    {
                        "mælk", "melk", "mjölk", "milch", "milk", "whole milk", "skimmed milk",
                        "milk powder", "mælkepulver", "milk protein", "mælkeprotein", "whey",
                        "valle", "cream", "fløde", "cheese", "ost", "butter", "smør", "lactose", "laktose"
                    }
                },

                // GLUTEN / WHEAT
                {
                    "gluten", new[]
                    {
                        "gluten", "wheat", "hvede", "hvete", "vete", "weizen", "hvedemel",
                        "durum", "semolina", "barley", "byg", "gerste", "malt", "bygmaltekstrakt",
                        "rye", "rug", "roggen", "råg", "oats", "havre", "hafer", "havregryn",
                        "spelt", "dinkel", "kamut", "couscous", "bulgur", "vital wheat gluten"
                    }
                },

                // TREE NUTS
                {
                    "tree nuts", new[]
                    {
                        "nuts", "nødder", "nötter", "nüsse", "almond", "mandel", "mandler",
                        "marzipan", "marcipan", "hazelnut", "hasselnød", "walnut", "valnød",
                        "cashew", "pistachio", "pistacie", "pecan", "brazil nut", "paranød",
                        "macadamia", "pine nut", "pinjekerne", "chestnut", "kastanje"
                    }
                },
                {
                    "nødder", new[]
                    {
                        "nødder", "nuts", "nötter", "nüsse", "mandel", "almond", "hasselnød",
                        "hazelnut", "valnød", "walnut", "cashew", "pistacie", "pecan",
                        "paranød", "macadamia", "pinjekerne", "kastanje", "marcipan"
                    }
                },

                // PEANUTS
                {
                    "peanut", new[]
                    {
                        "peanut", "peanuts", "jordnød", "jordnødder", "jordnöt", "erdnuss",
                        "arachis", "groundnut", "peanut butter", "peanut paste", "peanut oil"
                    }
                },
                {
                    "jordnød", new[]
                    {
                        "jordnød", "jordnødder", "jordnöt", "peanut", "peanuts", "groundnut",
                        "erdnuss", "peanut butter", "peanut oil"
                    }
                },

                // FISH & SHELLFISH
                {
                    "fisk", new[]
                    {
                        "fisk", "fish", "fisch", "tuna", "tun", "salmon", "laks", "cod", "torsk",
                        "herring", "sild", "mackerel", "makrel", "sardine", "anchovy", "trout",
                        "pollock", "fish oil", "fiskeolie", "surimi", "fiskeprotein"
                    }
                },
                {
                    "skaldyr", new[]
                    {
                        "skaldyr", "shellfish", "schalentiere", "reje", "rejer", "shrimp",
                        "prawn", "krabbe", "crab", "hummer", "lobster", "krebs", "crayfish",
                        "scampi", "langoustine", "crustacean"
                    }
                },

                // EGGS
                {
                    "æg", new[]
                    {
                        "æg", "æggehvide", "æggeblomme", "egg", "eggs", "egg white", "egg yolk",
                        "ägg", "ei", "albumin", "ovalbumin", "æggepulver", "whole egg"
                    }
                },

                // SOY
                {
                    "soja", new[]
                    {
                        "soja", "soy", "soya", "sojabønne", "sojabønner", "soybean",
                        "soy protein", "sojaprotein", "soy flour", "soy lecithin",
                        "sojalecithin", "tofu", "tempeh", "miso", "soy sauce", "sojasauce"
                    }
                },

                // VARIOUS (CITRUS, TOMATO, ONION, ETC.)
                { "mustard", new[] { "mustard", "sennep", "senap", "senf", "mustard seed", "mustard flour" } },
                { "sesame", new[] { "sesame", "sesam", "tahini", "tahin", "sesame oil", "sesame seeds" } },
                { "celery", new[] { "celery", "selleri", "celeriac", "sellerirod", "celery salt" } },
                { "sulfitter", new[] { "sulfitter", "sulfit", "sulphite", "sulfur dioxide", "svovldioxid", "e220", "e221" } },
                { "majs", new[] { "majs", "mais", "corn", "maize", "corn starch", "majsstivelse", "corn syrup" } },
                { "æble", new[] { "æble", "apple", "apfel", "pomme", "apple juice", "apple puree" } },
                { "banan", new[] { "banan", "banana", "bananpuré", "banana powder" } },
                { "citrus", new[] { "citrus", "orange", "appelsin", "lemon", "citron", "lime", "grapefruit" } },
                { "tomat", new[] { "tomat", "tomato", "pomodoro", "tomatpuré", "passata", "tomatsauce" } },
                { "hvidløg", new[] { "hvidløg", "garlic", "hvidløgspulver", "garlic powder" } },
                { "kakao", new[] { "kakao", "cocoa", "chocolate", "chokolade", "kakaomasse", "kakaosmør" } },
                { "glutamat", new[] { "monosodium glutamate", "msg", "e621", "natriumglutamat" } }
            };
    }
}
