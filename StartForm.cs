﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BlackJack
{
    partial class StartForm : Form
    {
        private BlackJackGame game;
        /// <summary>
        /// Main constructor for StartForm
        /// </summary>
        public StartForm()
        {
            InitializeComponent();
		}
		
        /// <summary>
        /// Invokes a new BlackJack game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void NewGameBtn_Click(object sender, EventArgs e)
        {
             game = new BlackJackGame(Properties.Settings.Default.InitBalance);
            //         // Show the main BlackJack UI game
            //using (BlackJackForm blackjackform1 = new BlackJackForm(game))
            //{
            //	Hide();
            //	blackjackform1.ShowDialog();
            //	Show();
            //}
            //         using (BlackJackForm blackjackform2 = new BlackJackForm(game))
            //         {
            //             Hide();
            //             blackjackform2.ShowDialog();
            //             Show();
            //         }
            //         using (BlackJackForm blackjackform3 = new BlackJackForm(game))
            //         {
            //             Hide();
            //             blackjackform3.ShowDialog();
            //             Show();
            //         }
            List<BlackJackForm> forms = new List<BlackJackForm>();
            game.gameState = BlackJackGame.GameState.PREFLOP;
            int j = 111;
            for (int i = 0; i < 3; i++)
            {
                
                forms.Add(new BlackJackForm(game));
                forms[i].FormIndex = i;
                forms[i].FormId = j;
                forms[i].Text = i.ToString();
                game.addPlayer(forms[i]);
                forms[i].ShowBankValue();
          

            }

            game.startGame();
           
        
            
            for (int i = 0; i < 3; i++)
            {
                forms[i].Show();
            }

            //BlackJackForm f1 = new BlackJackForm(game);
            //BlackJackForm f2 = new BlackJackForm(game);
            //BlackJackForm f3 = new BlackJackForm(game);
            //game.addPlayer(f1);
            //game.addPlayer(f2);
            //game.addPlayer(f3);
            //Hide();
            //f1.Show();   
            //f2.Show();
            //f3.Show();



        }

        /// <summary>
        /// Brings up the options form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void OptionsBtn_Click(object sender, EventArgs e)
        {
            // Show the Options panel
			using (OptionsForm optionsForm = new OptionsForm())
			{
				Hide();
				optionsForm.ShowDialog();
				Show();
			}
		}

        /// <summary>
        /// Exits the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void ExitBtn_Click(object sender, EventArgs e)
        {
            // Exit the Game
            Application.Exit();
		}
	}
}