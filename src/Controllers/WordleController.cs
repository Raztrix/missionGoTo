using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordleController : ControllerBase
    {
        WordleRepository _WordleRepository;
        private readonly IMemoryCache _memoryCache;

        public WordleController(WordleRepository wordleRepository, IMemoryCache memoryCache)
        {
            _WordleRepository = wordleRepository;
            _memoryCache = memoryCache;
        }
        /// <summary>
        /// Example of calls
        /// </summary>
        /// <returns>string</returns>
        [HttpGet]
        public async Task<ActionResult> Test()
        {
            _WordleRepository.CreateWordle();

            var isOkWord = _WordleRepository.IsAWord("Aalst");

            var response = _WordleRepository.CheckWord("abcde");

            return Ok(response);
        }

        //What to add 
        //the function should get customer identifier (could be anything)
        //this will be used in allow only 6 guess
        //Cache example in WordleRepository


        //function (not get) to start game
        [HttpPost("startGame")]
        public async Task<ActionResult> StartGame(string identifier)
        {
            if (identifier == "1234")
            {
                _WordleRepository.CreateWordle();
                return Ok("Pls guess word in the next api");
            }

            return BadRequest("The identifier is incorrect");
        }

        //function (not get) to check word exists in dictionary
        [HttpPost("checkWordExist")]
        public async Task<ActionResult> CheckIfWordExist(string identifier, string word)
        {
            if (identifier == "1234" && _memoryCache.Get("word") != null)
            {
                if (_WordleRepository.IsAWord(word))
                {
                    return Ok("Word exist you can continue to check the word vs the current word");
                }

                return BadRequest("Word is not exist");

            }
            return BadRequest("Identifier is incorrect or you have to start the game first!");
        }

        //function (not get) to check word against current word
        //please make sure that you return a better object then "01020"
        //Add support for swagger
        [HttpPost("checkWordVsCurrent")]
        public async Task<ActionResult> CheckWordVsCurrentWord(string identifier, string word)
        {
            if (identifier == "1234" && !string.IsNullOrEmpty(_memoryCache.Get<string>("word")) && word.Length == 5)
            {
                string CustomerWordResult = _WordleRepository.CheckWord(word);

                if (CustomerWordResult == "22222" && _memoryCache.Get<int>("numOfGuesses") != 0) // all the letters correct and num of guesses is not zero
                {
                    return Ok("WINNER!");
                }
                else if (CustomerWordResult.Contains('0') || CustomerWordResult.Contains('1') && _memoryCache.Get<int>("numOfGuesses") != 0) // word incorrect and num of guesses in not zero
                {
                    numberOfGuessesLeft();
                    if(_memoryCache.Get<int>("numOfGuesses") <= 0)
                    {
                        return Ok("You lost, runned out of guesses");
                    }
                    return Ok($"Incorrect word you left with {_memoryCache.Get<int>("numOfGuesses")} Guesses, and your word result is: {CustomerWordResult}, {_memoryCache.Get("word")}, \n 0 - means wrong letter, \n 1 - means right letter but in the wrong place, \n 2 - means right letter in the right place");
                }
            }

            return BadRequest("reasons to failure:\n * you have to start the game first \n * enter word with 5 letters \n * incorrect identifier");
        }


        //function here to allow only 6 guess - using cache by customer
        //so you need to store the number of guess
        private int numberOfGuessesLeft()
        {
            _memoryCache.Set<int>("numOfGuesses", _memoryCache.Get<int>("numOfGuesses") - 1);

            if (_memoryCache.Get<int>("numOfGuesses") > 0)
            {
                return _memoryCache.Get<int>("numOfGuesses");
            }
            return 0;
        }



        //all function should have validation + correct http status to return
        // 200, 403 if there is an error

    }
}
