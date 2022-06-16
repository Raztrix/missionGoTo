using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace src
{
    public class WordleRepository
    {
        private List<string> words;
        private string CurrentWord = string.Empty;
        private readonly IMemoryCache _memoryCache;
        public WordleRepository(IMemoryCache memoryCache)
        {
            words = new List<string>();
            FillItems();
            _memoryCache = memoryCache;
            _memoryCache.Set<int>("numOfGuesses", 6);
        }

        /// <summary>
        /// This one create a wordle item, will be up until exit from Debug (singleton from Services)
        /// </summary>
        /// <returns></returns>
        public bool CreateWordle()
        {
            //Example of Saving in Cache

            var word = _memoryCache.Get<string>("word");
            if (word == null)
            {
                var rnd = new Random();
                CurrentWord = words.Where(x => x.Length == 5).OrderBy(x => rnd.Next()).FirstOrDefault();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(3));

                _memoryCache.Set<string>("word", CurrentWord, cacheEntryOptions);
            }
            else
            {
                return true;
            }
            return true;
        }

        private string GetCurrentWord()
        {
            var word = _memoryCache.Get<string>("word");
            if (word == null)
            {
                var rnd = new Random();
                CurrentWord = words.Where(x => x.Length == 5).OrderBy(x => rnd.Next()).FirstOrDefault();
                word = CurrentWord;
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromHours(3));

                _memoryCache.Set<string>("word", CurrentWord, cacheEntryOptions);
            }
           
            return word;
        }

        /// <summary>
        /// This one check if word in in dictionary
        /// </summary>
        /// <param name="TestWord"></param>
        /// <returns></returns>
        public bool IsAWord(string TestWord)
        {
            return words.Any(word => word.Equals(TestWord));
        }

        /// <summary>
        /// 0 not 
        /// 1 right wrong place
        /// 2 right in place
        /// </summary>
        /// <param name="TestWord"></param>
        /// <returns></returns>
        public string CheckWord(string TestWord)
        {
            //code to check here

            var rtn = new char[5] { '0', '0', '0', '0', '0' };

            if (!string.IsNullOrEmpty(TestWord) && !string.IsNullOrEmpty(GetCurrentWord()))
            {
                TestWord = TestWord.ToLower();
                if (TestWord.Length == 5)
                {
                    //naive approch
                    var array = TestWord.ToCharArray();
                    var CurrentArray = CurrentWord.ToCharArray();
                    bool marked = false;
                    for (int i = 0; i < array.Length; i++)
                    {
                        var t = array[i];
                        marked = false;
                        //naive
                        for (int j = 0; j < CurrentArray.Length; j++)
                        {
                            var c = CurrentArray[j];
                            if (t == c)
                            {
                                marked = true;
                                if (i == j)
                                {//same place

                                    rtn[j] = '2';
                                }
                                else
                                {
                                    //do we need to check other letters ?
                                    //check if letter is marked (only once)
                                    if(rtn[i] == '2')
                                    {
                                        rtn[i] = '2';
                                    }
                                    else
                                    {
                                        rtn[i] = '1';
                                    }
                                 
                                }

                            }
                        }
                    }
                }
            }

            return string.Join("", rtn);
        }

        /// <summary>
        /// 23197 words
        /// </summary>
        private void FillItems()
        {
            try
            {
                var lines = File.ReadAllLines(@"Words.txt");
                foreach (var line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (line.Length == 5)
                        {
                            words.Add(line.ToLower());
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                //TOOD log here
                var a = exp;
            }
        }

    }

}


