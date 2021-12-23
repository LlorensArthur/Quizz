using System.Text.RegularExpressions;

namespace Quizz
{
    class Quizz
    {
        static List<Question> questions = new List<Question>();
        static List<Question, Answer> logs = new List<Question, Answer>();

        // Add the questions in the game from the questionTextFile.txt, then starts the game loop and displays the results.
        // The user can access the admin mode by typing ADMIN during the questions.
        static void Main(string[] args)
        {
            AddQuestionsFromText();
            GameLoop();
            DisplayResult();
        }
        // Read the questions from questionTextFile.txt and then creates Question objects filled with the answers. Add each question to a list that will be used in the GameLoop()
        // The format of the questions the text file is the following : [question]?[answer],[answer],...
        static void AddQuestionsFromText()
        {
            // The text file is located at the root of the program.
            string sAppPath = Environment.CurrentDirectory;
            string questionTextFile = sAppPath + "\\questionTextFile.txt";

            // Opens questionTextFile.txt that we will read the questions from.
            // All the questions has been formatted like this : [question]?[answer],[answer],...
            using (StreamReader sr = File.OpenText(questionTextFile))
            {
                string s;
                // Read all the line one per one. Store a line in the temp string s
                // Each question are represented in the programm as a Question class. Each question class contains the question string and a list of answer. An answer can be true or false.
                while ((s = sr.ReadLine()) != null)
                {
                    // Split the question and answers using ?. The question will be in the first value and all the answers will be in the second value
                    string[] questionAndAnswers = s.Split('?');
                    // Split all the answers using ,
                    string[] answers = questionAndAnswers[1].Split(',');

                    // The list of answer will be filled in the constructor of the Question class
                    List<Answer> answerList = new List<Answer>();
                    // Loops to create Answer objects
                    foreach (string answer in answers)
                    {
                        // An answer starting by the + sign is a true answer. The sign is deleted as only the program should know the right answer, not the user.
                        if (answer[0] == '+')
                        {
                            answerList.Add(new Answer(answer.Remove(0, 1), true));
                        }
                        else
                            answerList.Add(new Answer(answer, false));

                    }
                    // Creates a new question object with the question name and the filled objects
                    questions.Add(new Question(questionAndAnswers[0], answerList));
                    // Used for debug propose. Display the original list of the questions. 
                    Console.WriteLine(s);
                }
            }
        }

        // The main game loop will loop over all the question. For each question, the programm will let the player choose between the given questions by typing the anwer number.
        // User inputs are checked and asked back if incorrect. Checks are done by verifying the size of the input and regex.
        static void GameLoop()
        {
            // Loop over all the questions
            foreach (Question question in questions)
            {
                // Displays the question
                Console.WriteLine($"Question : {question.question}");
                // Displays all the possible answers with a number for each answer.
                for (int i = 0; i < question.answers.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {question.answers[i].answer}");
                }

                // Verification loop of the player input. Also access to the admin mode by typing ADMIN.
                // To select the answer, the player will have to type the number of the answer.
                // Anything that is not a number of a possible answer will be refused. Checks are done by verifying the size of the input and regex.
                string userInput = "";
                while (true)
                {
                    userInput = Console.ReadLine();
                    Regex isANumberBetween1and9 = new Regex(@"^[1-9]");
                    // Hidden admin mode.
                    // The admin mode can add a question at the end of all the questions or delete a question.
                    if (userInput == "ADMIN")
                    {
                        AdminMode();
                    }
                    // Check if the user input is a number of an answer, otherwise display an error message to the user.
                    else if (userInput.Length != 1 || !isANumberBetween1and9.IsMatch(userInput) || int.Parse(userInput) < 0 || int.Parse(userInput) > question.answers.Count)
                    {
                        Console.WriteLine($"Veuillez rentrer un chiffre entre 1 et {question.answers.Count}");
                    }
                    // If the answer is correct, breaks the verification loop.
                    else
                    {
                        break;
                    }
                }

                // Parse the user input from a string to an in that we use to get the answer from the list of answers of the Question object.
                int answerChosen = int.Parse(userInput) - 1;
                Answer loggedAnswer = question.answers[answerChosen];

                // Display a message to the user if the answer is true.
                // Uses a cool change of color.
                if (loggedAnswer.isTrue)
                {
                    Console.Write("Bravo ! c'est ");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("vrai");
                    Console.ResetColor();
                    Console.WriteLine(".");
                }
                // Display a message to the user if the answer is false.
                // Uses a cool change of color.
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("FAUX");
                    Console.ResetColor();
                    Console.WriteLine(".");
                }
                // Add the question and the user's answer to a log that we will display after the end of the loop.
                logs.Add(question, loggedAnswer);
                // Display to the user that we will display the next question.
                Console.WriteLine("Question suivante.");
            }
        }
        // After the game loop, the program displays the user's logs.
        // For each question andswered, displays the question, the user answer and if it's true or not.
        // Then, displays a final result.
        static void DisplayResult()
        {
            int result = 0;
            Console.WriteLine("Voici vos résultats : ");
            // For each question andswered, displays the question, the user answer and if it's true or not.
            for (int i = 0; i < logs.question.Count; i++)
            {
                Console.WriteLine($"Question n°{i} : {logs.question[i].question}.\n" +
                    $"Vous avez répondu {string.Join("\n", logs.userAnswer[i].answer)}" +
                    $", ce qui est {(logs.userAnswer[i].isTrue ? "vrai" : "faux")}.");
                // The result count gets iterated when the user answered a question correctly.
                if (logs.userAnswer[i].isTrue)
                {
                    result++;
                }
            }
            // Displays the final result.
            Console.WriteLine($"Vous avez obtenu {result} points sur {logs.question.Count}.");
        }

        // The hidden admin mode. Called of the user type ADMIN when answering a question.
        static void AdminMode()
        {
            Console.WriteLine($"Welcome to the admin mode. I hate you. :)");
            Console.WriteLine($"You have exited the game loop. The game will restart after your changes.\nAnd if you haven't changed anything? Too bad.");
            while (true)
            {
                Console.WriteLine($"What do you want to do? Add, remove or modify? Type Exit to exit. Obviously.");
                string userInput = Console.ReadLine();

                // The admin wants to add a question.
                // The program will ask the user the name of the questions, then, for each answer, the name if the answer and if it's a correct answer.
                // Then, this question and its answers will be written in the text file named questionTextFile.txt
                // Here is the format : [question]?[answer],[answer],... The true answer will have an +before the text.
                if (string.Equals(userInput, "Add", StringComparison.OrdinalIgnoreCase))
                {
                    string questionSentence = "";
                    List<Answer> answerList = new List<Answer>();
                    bool isTrueAnswer = false;

                    // Verification loop of the question.
                    do
                    {
                        Console.WriteLine("Please enter the question. You must finish by a ? \n Do not add a ? or a , in a sentence or you will have an error the program.");
                        questionSentence = Console.ReadLine();
                    }
                    while (!IsValidQuestion(questionSentence));
                    Console.WriteLine("You can enter N answers. One and only one has to be true. \n Do not add a ? or a , in a sentence or you will have an error the program.");
                    // Loop to add a N answers.
                    // The user can exit the loop by typing QUIT. If there is no true answer, the question will be incorrect and thus not written in the text file.
                    while (true)
                    {
                        string answerSentence;
                        answerSentence = "";

                        // Tell to the admin that they can only have one true answer. If a true answer has been put, remind it so that the admin knows where they are in the program.
                        Console.WriteLine(!isTrueAnswer ? "Please enter an answer" : "Please enter an answer. You have entered the only true answer already.");
                        // User verification loop for the answer.
                        // The user can add an answer as long as there is no , or ? in it since we use them later as separator when we read the text file.
                        // The user can type QUIT to exit the add function. This will save the question and its answers in the text file.
                        Console.WriteLine("Type QUIT to save the question. A question without a true answer will not be saved.");
                        do
                        {
                            Console.WriteLine("Answer must not have a ? or a , in them.");
                            answerSentence = Console.ReadLine();
                        }
                        while (!IsValidAnswer(answerSentence));
                        // This is where the user can type QUIT to exit the loop. This will save the question and its answers in the text file.
                        if (string.Equals(answerSentence, "QUIT", StringComparison.OrdinalIgnoreCase))
                        {
                            // If a true answer has been put, says to the user that the format is correct.
                            // If no true answer has been put, the displays a warning to the user.
                            Console.WriteLine(isTrueAnswer?"The new answer will be written in the text file."
                                :"You haven't put a true answer. The format is incorrect, thus the question will not be written");
                            break;
                        }
                        // Additional question that appears each time an answer is entered until a question has been set to true.
                        // The user must type yes to set the answer to true.
                        if (!isTrueAnswer)
                        {
                            while (true)
                            {
                                Console.WriteLine("To make this question the true answer type yes. Otherwise, press enter. There can be only one true answer per question.");
                                string makeItTrueSentence = Console.ReadLine();
                                if (string.Equals(makeItTrueSentence, "yes", StringComparison.OrdinalIgnoreCase))
                                {
                                    // We add the + before the answer that will make the answer true when the game is loaded with AddQuestionsFromText()
                                    answerSentence = answerSentence.Insert(0, "+");
                                    isTrueAnswer = true;
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        // Add an answer. Check if it's true by looking at the + sign.
                        // We have to tell it in the object as well because the a question object is filled and used as reference to write on the text file.
                        answerList.Add(new Answer(answerSentence, answerSentence[0] == '+' ? true : false));
                    }
                    // Skip the writing part to continue the loop as long as the true question has not been added
                    if (!isTrueAnswer)
                    {
                        continue;
                    }
                    // Creates the object with the question sentence and the list of filled answers.
                    Question newQuestion = new Question(questionSentence, answerList);
                    // The text file is located at the root of the program.
                    string sAppPath = Environment.CurrentDirectory;
                    string questionTextFile = sAppPath + "\\questionTextFile.txt";
                    string writeNewQuestion = "";
                    // Creates the question with the format : [question]?[answer],[answer],... The true answer is the one with the + before the text.
                    writeNewQuestion = ($"{newQuestion.question}")
                        + (string.Join(',', answerList.Select(x => x.answer))
                        + ("\n"));
                    // Writes the question at the end of the text file on a new line.
                    File.AppendAllText(questionTextFile, writeNewQuestion);
                }
                // The admin wants to remove a question.
                // Display each question in questionTextFile.txt with a number. Then, the user can type the number of the question to remove it. 
                // Techically, in order :
                // 1- A file is created in a temp position.
                // 2- This file copies all the questions except the selected question.
                // 3- The temp file replaces the original questionTextFile.txt.
                else if (string.Equals(userInput, "Remove", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Which question do you want to remove?");

                    // The text file is located at the root of the program.
                    string sAppPath = Environment.CurrentDirectory;
                    string questionTextFile = sAppPath + "\\questionTextFile.txt";

                    // Display each question in questionTextFile.txt with a number. 
                    using (StreamReader sr = File.OpenText(questionTextFile))
                    {
                        string s;
                        int i = 0;
                        while ((s = sr.ReadLine()) != null)
                        {
                            string question = s.Split('?')[0];
                            Console.WriteLine($"{i}. {question}");
                            i++;
                        }
                    }
                    // The input of the user will be parsed as line index for the question to be removed, meaning not copied in the new questionTextFile.txt
                    int lineToBeRemoved = int.Parse(Console.ReadLine());
                    string tempFile = "tempFile.txt";
                    // 1- A file is created in a temp position.
                    using (var sr = new StreamReader(questionTextFile))
                    using (var sw = new StreamWriter(tempFile))
                    {
                        string line;
                        // 2- This file copies all the questions except the selected question.
                        int i = 0;
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (i != lineToBeRemoved)
                                sw.WriteLine(line);
                            i++;
                        }
                    }

                    // 3- The temp file replaces the original questionTextFile.txt.
                    File.Delete(questionTextFile);
                    File.Move(tempFile, questionTextFile);
                }
                else if (string.Equals(userInput, "Exit", StringComparison.OrdinalIgnoreCase))
                {
                    questions = new List<Question>();
                    logs = new List<Question, Answer>();
                    Console.Clear();
                    Main(null);
                }
                else
                {
                    Console.WriteLine("I haven't understood your input. Are you sure that you deserve to be an admin?");
                }
            }
        }

        static bool IsValidQuestion(string question)
        {
            Regex endByQuestionMark = new Regex(@"[\?]$");
            Regex hasCommas = new Regex(@"[,]");
            return endByQuestionMark.IsMatch(question) && !hasCommas.IsMatch(question);
        }
        static bool IsValidAnswer(string answer)
        {
            Regex hasCommasOrQuestionMarks = new Regex(@"[,\,?]");
            return !hasCommasOrQuestionMarks.IsMatch(answer);
        }
    }
}
