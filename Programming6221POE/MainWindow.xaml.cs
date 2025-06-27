using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Speech.Synthesis;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace ChatBotProgramPart2
{
    public partial class MainWindow : Window
    {
        static string userFavouriteTopic = null;
        static List<string> chatHistory = new List<string>();
        static SpeechSynthesizer synth = new SpeechSynthesizer();

        class TaskItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime? ReminderTime { get; set; }
            public bool IsCompleted { get; set; }
        }

        static List<TaskItem> tasks = new List<TaskItem>();

        string userName;

        public MainWindow()
        {
            InitializeComponent();
            PlayGreetingAudio(@"C:\Users\braed\source\repos\ProgrammingPOE\ProgrammingPOE\Audio\Welcome.wav");
            string asciiArt = @"

   _______     ______  ______ _____    _      _____ ______ ______ 
  / ____\ \   / |  _ \|  ____|  __ \  | |    |_   _|  ____|  ____|
 | |     \ \_/ /| |_) | |__  | |__) | | |      | | | |__  | |__   
 | |      \   / |  _ <|  __| |  _  /  | |      | | |  __| |  __|  
 | |____   | |  | |_) | |____| | \ \  | |____ _| |_| |    | |____ 
  \_____|  |_|  |____/|______|_|  \_\ |______|_____|_|    |______|


                                                                  ";


            userName = Microsoft.VisualBasic.Interaction.InputBox("Enter your name:", "Welcome", "Joe");
            if (string.IsNullOrWhiteSpace(userName)) userName = " Joe";

            AppendChat("Chatbot", $"Hello, {userName}!");
            RespondWithSpeech($"Hello, {userName}!");
            AppendChat("Chatbot", "You can ask any questions related to CyberSecurity. Use help for guide, exit to close app, add task, view tasks, delete task, view favourite topic or ask for phishing tips.");

            RespondWithSpeech("You can ask any questions related to CyberSecurity. Use help for guide, exit to close app, add task, view tasks, delete task, view favourite topic or ask for phishing tips.");
        }

        private void PlayGreetingAudio(string filePath)
        {
            try
            {
                if (File.Exists(filePath)) new SoundPlayer(filePath).Play();
                else AppendChat("Chatbot", $"Error: The audio file '{filePath}' was not found.");
            }
            catch (Exception ex)
            {
                AppendChat("Chatbot", $"Error playing audio: {ex.Message}");
            }
        }

        private void AppendChat(string sender, string message)
        {
            string formattedMessage;

            if (!string.IsNullOrEmpty(sender))
                formattedMessage = $"{sender}: {message}";
            else
                formattedMessage = message;

            if (!string.IsNullOrEmpty(ChatHistoryTextBox.Text))
                ChatHistoryTextBox.Text += Environment.NewLine;

            ChatHistoryTextBox.Text += formattedMessage;
            ChatHistoryTextBox.ScrollToEnd();

            chatHistory.Add(formattedMessage);
        }



        private void RespondWithSpeech(string response)
        {
            try { synth.SpeakAsync(response); }
            catch (Exception ex) { AppendChat("Chatbot", $"Speech error: {ex.Message}"); }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) => ProcessUserInput();
        private void UserInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessUserInput();
                e.Handled = true;
            }
        }

        private void ProcessUserInput()
        {
            string input = UserInputTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(input)) return;
            AppendChat(userName, input);
            UserInputTextBox.Clear();

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase)) { AppendChat("Chatbot", "Thank you for using the Cyber Security ChatBot."); RespondWithSpeech("Thank you for using the Cyber Security ChatBot."); Application.Current.Shutdown(); return; }
            if (input.Equals("history", StringComparison.OrdinalIgnoreCase)) { ShowHistory(); return; }
            if (input.Contains("worried") || input.Contains("happy") || input.Contains("sad") || input.Contains("anxious")) { Emp(input); return; }
            if (input.Contains("interested")) { Favourite(input); return; }
            if (input.Contains("favourite") || input.Contains("favorite")) { ShowFavourite(); return; }
            if (input.Contains("add task")) { AddTask(); return; }
            if (input.Contains("view tasks") || input.Contains("view task")) { ViewTasks(); return; }
            if (input.Contains("delete task")) { DeleteTask(); return; }
            if (input.Contains("play quiz") || input.Contains("quiz game")) { StartQuiz(); return; }
            BreakUpLine(input);
        }

        private void BreakUpLine(string input)
        {
            Dictionary<string, string> keywords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"privacy","privacy" }, { "password", "password" }, { "phishing tips", "phishing tips" },
                { "phishing", "phishing" }, { "help", "help" }, { "browse", "browse" }, { "purpose", "purpose" },
                { "vpn", "vpn" }, { "malware", "malware" }, { "firewall", "firewall" }, { "2fa", "2fa" },
                { "ransomware", "ransomware" }, { "spyware", "spyware" }, { "social engineering", "social engineering" },
                { "updates", "updates" }, { "antivirus", "antivirus" }, { "email security", "email security" },
                { "public wifi", "public wifi" }, { "data breach", "data breach" }, { "how are you", "How are you" }
            };

            foreach (var kvp in keywords)
            {
                if (input.ToLower().Contains(kvp.Key.ToLower())) { HandleUserQuery(kvp.Value); return; }
            }

            HandleUserQuery(input);
        }

        private void HandleUserQuery(string input)
        {
            Dictionary<string, string> responses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"privacy", "Protect your personal data and share it only with trusted sources." },
                {"help", "You can ask questions like: How to create a secure password?" },
                {"password", "Use complex, long passwords like 'k8dfh8c@Pfv0gB2' or 'HorsePurpleHatRunBay'." },
                {"phishing", "Phishing is when attackers trick you into providing sensitive information." },
                {"browse", "Use HTTPS, update browsers, clear cookies, use VPN, and avoid unsafe downloads." },
                {"How are you", "I'm good, thank you! Ready to help you." },
                {"purpose", "I help with all your Cyber Security questions." },
                {"vpn", "VPN hides your IP and encrypts your traffic, especially on public Wi-Fi." },
                {"malware", "Malware is harmful software like viruses or ransomware." },
                {"firewall", "A firewall blocks unauthorized traffic and protects your system." },
                {"2fa", "2FA adds extra security by requiring a second verification step." },
                {"ransomware", "Ransomware locks your files until you pay a ransom." },
                {"spyware", "Spyware secretly collects your personal information." },
                {"social engineering", "Social engineering tricks people into giving up confidential information." },
                {"updates", "Always install updates to fix security vulnerabilities." },
                {"antivirus", "Antivirus software helps detect and remove threats." },
                {"email security", "Don’t click unknown links and always use strong email passwords." },
                {"public wifi", "Avoid entering sensitive info over public Wi-Fi or use a VPN." },
                {"data breach", "A data breach happens when your info is stolen. Change passwords immediately." }
            };

            if (responses.ContainsKey(input)) { AppendChat("Chatbot", responses[input]); RespondWithSpeech(responses[input]); }
            else if (input.ToLower().Contains("phishing tips")) { PhishingTips(); }
            else { AppendChat("Chatbot", $"Sorry I didnt quite get that{userName}, try using words in your sentance that are related to your problem?"); RespondWithSpeech($"Sorry I didnt quite get that{userName}, try using words in your sentance that are related to your problem?"); }
        }

        private void PhishingTips()
        {
            string[] tips =
            {
                "Be cautious with email links — hover to verify URLs.",
                "Never share passwords or credit card info via email.",
                "Check sender addresses carefully.",
                "Look out for spelling mistakes and generic greetings.",
                "Enable two-factor authentication.",
                "Don’t open unexpected attachments.",
                "Keep your system and antivirus updated.",
                "Report suspicious emails to your IT department."
            };
            string randomTip = tips[new Random().Next(tips.Length)];
            AppendChat("Chatbot", randomTip);
            RespondWithSpeech(randomTip);
        }

        private void Favourite(string input)
        {
            foreach (var word in input.Split(' '))
            {
                if (word.Length > 3 && !word.Contains("interested"))
                {
                    userFavouriteTopic = word;
                    AppendChat("Chatbot", $"Got it! You're interested in {userFavouriteTopic}.");
                    RespondWithSpeech($"Got it! You're interested in {userFavouriteTopic}.");
                    return;
                }
            }
            AppendChat("Chatbot", "I'm not sure what your favorite topic is. Please try again.");
            RespondWithSpeech("I'm not sure what your favorite topic is. Please try again.");
        }

        private void ShowFavourite()
        {
            if (!string.IsNullOrEmpty(userFavouriteTopic))
                AppendChat("Chatbot", $"Your favourite topic is {userFavouriteTopic}.");
            else
                AppendChat("Chatbot", "You haven't told me your favourite topic yet.");
        }

        private void Emp(string input)
        {
            if (input.Contains("worried")) AppendChat("Chatbot", "I'm sorry you're feeling worried. Let's explore Cyber Security together.");
            else if (input.Contains("sad")) AppendChat("Chatbot", "Sorry you're feeling sad. Let's learn something interesting.");
            else if (input.Contains("anxious")) AppendChat("Chatbot", "It's okay to feel anxious. I'm here to help.");
            else if (input.Contains("happy")) AppendChat("Chatbot", "Great to hear you're happy! Let's keep learning.");
        }

        private void AddTask()
        {
            string title = Microsoft.VisualBasic.Interaction.InputBox("Enter task title:", "Add Task", "");
            if (string.IsNullOrWhiteSpace(title)) return;

            string description = Microsoft.VisualBasic.Interaction.InputBox("Enter task description:", "Add Task", "");
            TaskItem task = new TaskItem { Title = title, Description = description, IsCompleted = false };
            tasks.Add(task);
            AppendChat("Chatbot", $"Task '{title}' added successfully.");
        }

        private void ViewTasks()
        {
            if (tasks.Count == 0) { AppendChat("Chatbot", "No tasks available."); return; }
            int index = 1;
            foreach (var task in tasks)
            {
                string status = task.IsCompleted ? "[Completed]" : "[Pending]";
                AppendChat("Chatbot", $"{index}. {task.Title} - {task.Description} {status}");
                index++;
            }
        }

        private void DeleteTask()
        {
            if (tasks.Count == 0) { AppendChat("Chatbot", "No tasks to delete."); return; }
            string input = Microsoft.VisualBasic.Interaction.InputBox("Enter task number to delete:", "Delete Task", "");
            if (int.TryParse(input, out int number) && number > 0 && number <= tasks.Count)
            {
                string removedTitle = tasks[number - 1].Title;
                tasks.RemoveAt(number - 1);
                AppendChat("Chatbot", $"Task '{removedTitle}' deleted successfully.");
            }
            else AppendChat("Chatbot", "Invalid task number.");
        }

        public class QuizQuestion
        {
            public string QuestionText { get; set; }
            public List<string> Options { get; set; } // For multiple choice
            public int CorrectOptionIndex { get; set; } // Index of correct answer in Options list
            public string Explanation { get; set; }
            public bool IsTrueFalse { get; set; } // true if question is True/False type
        }

      
        private void StartQuiz()
        {
            List<QuizQuestion> quizQuestions = new List<QuizQuestion>()
    {
        new QuizQuestion {
            QuestionText = "What should you do if you receive an email asking for your password?",
            Options = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
            CorrectOptionIndex = 2,
            Explanation = "Correct! Reporting phishing emails helps prevent scams.",
            IsTrueFalse = false
        },
        new QuizQuestion {
            QuestionText = "True or False: Using the same password for multiple accounts is safe.",
            Options = new List<string> { "True", "False" },
            CorrectOptionIndex = 1,
            Explanation = "False! Using the same password increases risk if one account is compromised.",
            IsTrueFalse = true
        },
        new QuizQuestion {
            QuestionText = "What is two-factor authentication (2FA)?",
            Options = new List<string> { "Using two passwords", "A second verification step", "Ignoring suspicious emails", "Updating software" },
            CorrectOptionIndex = 1,
            Explanation = "2FA adds an extra layer of security beyond just a password.",
            IsTrueFalse = false
        },
        new QuizQuestion {
            QuestionText = "True or False: It's safe to connect to public Wi-Fi without a VPN.",
            Options = new List<string> { "True", "False" },
            CorrectOptionIndex = 1,
            Explanation = "False! Public Wi-Fi can be insecure; use a VPN to protect your data.",
            IsTrueFalse = true
        },
        new QuizQuestion {
            QuestionText = "Which of the following is a sign of a phishing email?",
            Options = new List<string> { "Spelling mistakes", "Known sender address", "Personalized greeting", "Secure website links" },
            CorrectOptionIndex = 0,
            Explanation = "Spelling mistakes often indicate phishing attempts.",
            IsTrueFalse = false
        },
        new QuizQuestion {
            QuestionText = "True or False: You should click on links from unknown senders.",
            Options = new List<string> { "True", "False" },
            CorrectOptionIndex = 1,
            Explanation = "False! Clicking unknown links can lead to malware or scams.",
            IsTrueFalse = true
        },
        new QuizQuestion {
            QuestionText = "What does a firewall do?",
            Options = new List<string> { "Blocks unauthorized network traffic", "Encrypts your files", "Deletes viruses", "Backs up data" },
            CorrectOptionIndex = 0,
            Explanation = "A firewall blocks suspicious or unauthorized traffic to protect your device.",
            IsTrueFalse = false
        },
        new QuizQuestion {
            QuestionText = "True or False: Updating your software regularly helps keep your system secure.",
            Options = new List<string> { "True", "False" },
            CorrectOptionIndex = 0,
            Explanation = "True! Updates patch vulnerabilities and improve security.",
            IsTrueFalse = true
        },
        new QuizQuestion {
            QuestionText = "Which is the safest way to create passwords?",
            Options = new List<string> { "Simple words", "Your birthday", "Long and complex combinations", "Reuse old passwords" },
            CorrectOptionIndex = 2,
            Explanation = "Long, complex passwords are the safest.",
            IsTrueFalse = false
        },
        new QuizQuestion {
            QuestionText = "True or False: Social engineering tricks people into giving confidential info.",
            Options = new List<string> { "True", "False" },
            CorrectOptionIndex = 0,
            Explanation = "True! Social engineering manipulates people, not systems.",
            IsTrueFalse = true
        }
    };

            int score = 0;

            foreach (var q in quizQuestions)
            {
                string questionMessage = q.QuestionText + Environment.NewLine;

                for (int i = 0; i < q.Options.Count; i++)
                {
                    questionMessage += $"{i + 1}) {q.Options[i]}" + Environment.NewLine;
                }

                string userAnswer = Microsoft.VisualBasic.Interaction.InputBox(questionMessage, "Cybersecurity Quiz", "");

                if (int.TryParse(userAnswer, out int answerIndex))
                {
                    answerIndex -= 1; // zero based index
                    if (answerIndex == q.CorrectOptionIndex)
                    {
                        MessageBox.Show("Correct! " + q.Explanation, "Feedback", MessageBoxButton.OK, MessageBoxImage.Information);
                        score++;
                    }
                    else
                    {
                        MessageBox.Show($"Wrong! {q.Explanation}", "Feedback", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid answer. Moving to next question.", "Warning", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }

            string finalFeedback;

            if (score == quizQuestions.Count)
                finalFeedback = $"Perfect! You scored {score}/{quizQuestions.Count}. You're a cybersecurity pro!";
            else if (score >= quizQuestions.Count * 0.7)
                finalFeedback = $"Great job! You scored {score}/{quizQuestions.Count}. Keep learning to stay safe online!";
            else
                finalFeedback = $"You scored {score}/{quizQuestions.Count}. Keep learning to stay safe online!";

            MessageBox.Show(finalFeedback, "Quiz Finished", MessageBoxButton.OK, MessageBoxImage.Information);

            AppendChat("Chatbot", $"Quiz finished! Your score: {score}/{quizQuestions.Count}");
            RespondWithSpeech(finalFeedback);
        }
        private void Dots(int count = 3, int delayMs = 300)
        {
            for (int i = 0; i < count; i++)
            {
                AppendChat("Chatbot", ".");
                Thread.Sleep(delayMs);
            }
        }
        private void ShowHistory()
        {

            ChatHistoryTextBox.Text += Environment.NewLine + "----- Conversation History -----" + Environment.NewLine;

            if (chatHistory.Count == 0)
            {
                ChatHistoryTextBox.Text += "No conversation history found." + Environment.NewLine;
            }
            else
            {
                foreach (var entry in chatHistory)
                {
                    ChatHistoryTextBox.Text += entry + Environment.NewLine;
                }
            }

            ChatHistoryTextBox.Text += "-------------------------------" + Environment.NewLine;
            ChatHistoryTextBox.ScrollToEnd();

            RespondWithSpeech("Here is your conversation history.");
        }





        private void ShowHistoryButton_Click(object sender, RoutedEventArgs e) => ShowHistory();
        private void AddTaskButton_Click(object sender, RoutedEventArgs e) => AddTask();
        private void ViewTasksButton_Click(object sender, RoutedEventArgs e) => ViewTasks();
        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e) => DeleteTask();
        private void PhishingTipButton_Click(object sender, RoutedEventArgs e) => PhishingTips();

        private void Quiz_Click(object sender, RoutedEventArgs e) => StartQuiz();
        private void ExitButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();
    }
}
