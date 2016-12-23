using System.Collections.Generic;
using BlackJack.CardGameFramework;
using System.Linq;

namespace BlackJack
{
	public class BlackJackGame
	{
		#region Fields
        
        // private Deck and Player objects for the current deck, dealer, and player
		private Deck deck;
		private Player dealer;
        private Player player;
        private List<Player> players;
        private Player table;
        
        #endregion

        #region Properties

        // public properties to return the current player, dealer, and current deck
        public Player CurrentPlayer { get { return player; } }
        public List<BlackJackForm> PlayerForms { get; set; }
        public List<BlackJackForm> PlayerFormsController { get; set; }
        public List<BlackJackForm> TurnPlayerForm { get; set; }
        public Player Dealer { get { return dealer; } }
        public Deck CurrentDeck { get { return deck; } }
        public bool ReBet { get; set; }
        public List<Player> Players { get { return players; } }
        public List<Card> TableCards { get; set; }
        public List<int> FoldedPlayers { get; set; }
        public int FormTurn { get; set; }
        public enum GameState {PREFLOP,FLOP,TURN,RIVER,ENDGAME};
        public decimal BetAmount { get; set; }
        public decimal MinBetAmount { get; set; }
        public bool Folded { get; set; }
        public GameState gameState { get; set; }
        #endregion

        #region Main Game Constructor

        /// <summary>
        /// Main Constructor for BlackJack Game
        /// </summary>
        /// <param name="initBalance"></param>
        public BlackJackGame(int initBalance)
		{
            Folded = false;
            BetAmount = 0;
            FormTurn = 0;
            // Create a dealer and one player with the initial balance.
            TableCards = new List<Card>();
            FoldedPlayers = new List<int>();
            dealer = new Player();
            player = new Player(initBalance) ;
            players = new List<Player>();
            PlayerForms = new List<BlackJackForm>();
            TurnPlayerForm = new List<BlackJackForm>();
            PlayerFormsController = new List<BlackJackForm>();
            table = new Player();

        }
        public Player getPlayerByFormId(int FormId)
        {
            return Players.FirstOrDefault(x => FormId == x.PlayerId);
        }
       public void GameOver()
        {
            foreach (var fplayer in FoldedPlayers)
                Players.Remove(getPlayerByFormId(fplayer));
            foreach (Player p in Players)
                p.Hand.GetValueOfHand(TableCards);
            players.Sort();
            foreach(BlackJackForm form in PlayerFormsController)
            {
                form.SetTextBox("Player " + players[players.Count-1].PlayerIndex + " has Won with " + players[players.Count - 1].Hand.GetValueOfHand(TableCards).ToString());
            }

            players[players.Count - 1].Balance += BetAmount;
            //PlayerForms[players[players.Count - 1].PlayerIndex].ShowBankValue();
           
            WinnerForm().ShowBankValue();
            foreach (var form in PlayerForms)
                form.LockControls();
        }
        public BlackJackForm WinnerForm()
        {
            BlackJackForm result=null;
            foreach (var form in PlayerForms)
            {
                if (players[players.Count - 1].PlayerId == form.FormId)
                    result= form;
            }
            return result;
        }
        public void addPlayer(BlackJackForm form)
        {
            this.PlayerForms.Add(form);
            PlayerFormsController.Add(form);
            this.TurnPlayerForm.Add(form);
            Player p = new Player(1000);
            p.PlayerIndex = form.FormIndex;
            p.PlayerId = form.FormId;
            this.players.Add(p);
        }
        #endregion

        #region Game Methods
        public void startGame()
        {
            DealNewGame();
            foreach (BlackJackForm form in PlayerForms)
            {
                form.UpdateUIPlayerCards();
            }
            foreach (BlackJackForm form in PlayerForms)
            {
                form.SetTextBox(gameState.ToString());
                if (FormTurn == form.FormIndex)
                    form.SetUpNewGame();
                else
                    form.LockControls();
            }

            FormTurn++;
        }


        public void SetUpGameInPlay()
        {
            var flag = false;
            BlackJackForm[] Forms= new BlackJackForm[3]; 
                TurnPlayerForm.CopyTo(Forms);
            if (!Folded)
            {
                TurnPlayerForm.RemoveAt(0);
                
            }
            Folded = false;
            if(Forms[0]!=null)
            Forms[0].LockControls();
           // Forms[0].ShowMin();
            if (TurnPlayerForm.Count>0)
            {

                foreach (BlackJackForm form in TurnPlayerForm)
                {
                    if (!flag)
                    {
                        form.ShowMin();
                        form.SetUpNewGame();
                        flag = true;
                    }
                    else
                        form.LockControls();

                }
            }
            else
            {
                gameState++;
                MinBetAmount = 0;
                foreach (Player p in players)
                    p.ClearBet();
                // FormTurn = 0;
                TurnPlayerForm = new List<BlackJackForm>(PlayerForms);
                 //if(gameState == BlackJackGame.GameState.TURN)// in order to add another gambling round you need to this
                //TurnPlayerForm.AddRange(PlayerForms);
                var first = TurnPlayerForm[0];
                int j = 0;
                foreach (BlackJackForm form in TurnPlayerForm)
                {
                    form.FormIndex = j;
                        j++;
                    form.SetTextBox(gameState.ToString());
                    form.ShowBankValue();
                    if (first == form)
                        form.SetUpNewGame();
                    else
                        form.LockControls();
                }
                switch (gameState)
                {
                   
                    case BlackJackGame.GameState.FLOP:
                        // gameOverTextBox.Text = "Flop";
                        for (int i = 0; i < 3; i++)
                        {
                            Card flopCard = CurrentDeck.Draw();
                            TableCards.Add(flopCard);

                        }


                        break;
                    case BlackJackGame.GameState.TURN:
                        // gameOverTextBox.Text = "Turn";
                        Card turnCard = CurrentDeck.Draw();
                        TableCards.Add(turnCard);

                        break;
                    case BlackJackGame.GameState.RIVER:
                        // gameOverTextBox.Text = "River";
                        Card riverCard = CurrentDeck.Draw();
                        TableCards.Add(riverCard);
                        

                        break;
                    case BlackJackGame.GameState.ENDGAME:
                        GameOver();
                        break;

                        //case BlackJackGame.GameState.NewStage: // To Add another level of the game you need to add another enum on gamestate & to the above switch case
                        //    Card newStage = CurrentDeck.Draw();
                        //    TableCards.Add(newStage);
                        //    break;


                }
                foreach (BlackJackForm form in PlayerForms)
                {
                    switch (gameState)
                    {
                        case BlackJackGame.GameState.FLOP:
                            
                            form.updateTableCards();
                            break;
                        case BlackJackGame.GameState.TURN:
                           

                            form.updateTableCards();
                            break;
                        case BlackJackGame.GameState.RIVER:
                            

                            form.updateTableCards();
                            
                            break;
                            // To Add another level of the game you need to add another enum on gamestate & to the above switch case
                    }
                }
              


            }
          
        }
       
        /// <summary>
        /// Deals a new game.  This is invoked through the Deal button in BlackJackForm.cs
        /// </summary>
        public void DealNewGame()
		{
            // Create a new deck and then shuffle the deck
            deck = new Deck();
            deck.Shuffle();

            // Reset the player and the dealer's hands in case this is not the first game
            foreach (Player p in Players)
            {
                p.NewHand();
            }
        

          
			// Deal two cards to each person's hand
            for(int p =0;p<players.Count;p++)
            {
                for (int i = 0; i < 2; i++)
                {
                    Card c = deck.Draw();
                    players[p].Hand.Cards.Add(c);

                   

                }
            }
            

            // Give the player and the dealer a handle to the current deck
           // player.CurrentDeck = deck;
            //dealer.CurrentDeck = deck;
		}

        /// <summary>
        /// This method finishes playing the dealer's hand
        /// </summary>
        public void DealerPlay()
        {
            // Dealer's card that was facing down is turned up when they start playing
            dealer.Hand.Cards[1].IsCardUp = true;

            // Check to see if dealer has a hand that is less than 17
            // If so, dealer should keep hitting until their hand is greater or equal to 17
            //if (dealer.Hand.GetSumOfHand() < 17)
            //{
            //    dealer.Hit();
            //    DealerPlay();
            //}
        }

        /// <summary>
        /// Update the player's record with a loss
        /// </summary>
        public void PlayerLose()
        {
            player.Losses += 1;
        }

        /// <summary>
        /// Update the player's record with a win
        /// </summary>
        public void PlayerWin()
        {
            player.Balance += player.Bet * 2;
            player.Wins += 1;
        }
		#endregion
	}
}
