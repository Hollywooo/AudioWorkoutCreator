﻿using AudioWorkoutCreator.Models;
using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Windows.Forms;
using Wavify.Core.Actions;

namespace AudioWorkoutCreator
{
    public partial class AudioWorkoutCreator : Form
    {
        private List<Exercise> _workout = new List<Exercise>();
        private string _savePath = "";

        public AudioWorkoutCreator()
        {
            InitializeComponent();
        }

        private void AddToWorkoutButton_Click(object sender, EventArgs e)
        {
            if(ExerciseBoxesFilled())
            {
                var exercise = new Exercise
                {
                    Name = ExerciseNameTextBox.Text,
                    Reps = RepsTextBox.Text,
                    Weight = WeightTextBox.Text,
                    SetTime = (int) SetTimeNumeric.Value,
                    RestTime = (int) RestTimeNumeric.Value
                };

                ExerciseListBox.Items.Add($"{exercise.Name} - {exercise.Weight} X {exercise.Reps} (ST: {exercise.SetTime}) (RT: {exercise.RestTime})");
                _workout.Add(exercise);
                WeightTextBox.Focus();
            }
        }

        private void SaveAsAudioButton_Click(object sender, EventArgs e)
        {
            if(ExerciseListBoxPopulated())
            {
                var speechPrompt = new PromptBuilder();

                speechPrompt.AppendText("Generated by Workout Audio Assistant. Please visit Streets of smashville dot com to download.");
                AppendBreakToPrompt(speechPrompt, 0, 3);
                speechPrompt.AppendText($"Welcome to Workout Assistant. Today you will be performing {ExerciseListBox.Items.Count} exercises.");
                AppendBreakToPrompt(speechPrompt, 0, 3);

                foreach (var exercise in _workout)
                {
                    speechPrompt.AppendText($"Please get in position for exercise, {_workout[0].Name} for {_workout[0].Reps} reps at {_workout[0].Weight} pounds.");
                    AppendBreakToPrompt(speechPrompt, 0, 15);
                    speechPrompt.AppendText($"Time for  {exercise.Name} for {exercise.Reps} reps at {exercise.Weight} pounds.");
                    speechPrompt.AppendText($"You will have {exercise.SetTime} seconds to complete the set.");
                    AppendBreakToPrompt(speechPrompt, 0, 5);
                    speechPrompt.AppendText($"Begin {exercise.Name}");
                    AppendBreakToPrompt(speechPrompt, 0, (exercise.SetTime / 2));
                    speechPrompt.AppendText("Half set time.");
                    AppendBreakToPrompt(speechPrompt, 0, (exercise.SetTime / 2));
                    speechPrompt.AppendText($"Good set! Time to rest. You will have {exercise.RestTime} to rest.");
                    AppendBreakToPrompt(speechPrompt, 0, (exercise.RestTime / 2));
                    speechPrompt.AppendText("Half rest time.");
                    AppendBreakToPrompt(speechPrompt, 0, (exercise.RestTime / 2));
                    speechPrompt.AppendText("Rest time is over.");
                }

                speechPrompt.AppendText("Awesome workout! You are a step closer to achieving your goals! See you next time!");

                SpeechAction.ConvertSpeechSynthPromptToMp3File(speechPrompt, _savePath);
            }
            
        }

        private bool ExerciseBoxesFilled()
        {
            return
                !String.IsNullOrEmpty(ExerciseNameTextBox.Text) &&
                !String.IsNullOrEmpty(WeightTextBox.Text) &&
                !String.IsNullOrEmpty(RepsTextBox.Text) &&
                SetTimeNumeric.Value > 0 &&
                RestTimeNumeric.Value > 0;
        }

        private bool ExerciseListBoxPopulated()
        {
            return ExerciseListBox.Items.Count > 0;
        }
       
        private PromptBuilder AppendBreakToPrompt(PromptBuilder prompt, int minutes, int seconds)
        {
            prompt.AppendBreak(new TimeSpan(0, minutes, seconds));
            return prompt;
        }

        private void SelectSaveLocationButton_Click(object sender, EventArgs e)
        {
            var saveFileName = $"Workout Assistant - {DateTime.Now.ToString("yyyy-MM-dd hh_mm_ss_tt")}.mp3";
            SaveFileDialog.FileName = saveFileName;
            
            DialogResult result = SaveFileDialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                SaveAsAudioButton.Visible = true;
                SaveLocationTextBox.Text = SaveFileDialog.FileName;
                _savePath = SaveFileDialog.FileName;
            }
        }
    }
}
