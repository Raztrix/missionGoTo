# Interview Question - assignment

we are testing creating a new web api return in .Net 6 to return a simple wordle repository 

The project is written in C# .Net 6 running WEB asp.net web api

How do you do a Wordle?

Wordle is simple: You have six chances to guess the day's secret five-letter word. Type in a word as a guess, and the game tells you which letters are or aren't in the word. The aim is to figure out the secret word with the fewest guesses.
## what we need

working in WordleController.cs file


* ### http method - start game
the function should not be get


* ### http method - check if word exists in dictionary 
the function should not be get

* ### http method - check word against the current word
1. this will be used in allow only 6 guess
2. please make sure that you return a better object then "01020"
3. Cache example in WordleRepository
4. function here to allow only 6 guess - using cache by customer
5. so you need to store the number of guess
6. the function should get customer identifier (could be anything)

##### all function should have swagger support
##### all function should have validation + correct http status to return 200, 403 if there is an error



**Examples in the simple http get**
