using System;
using System.Collections.Generic;
using System.IO;
using UnityThreading;
using UnityEngine;

namespace Common.Logger {
	/**
	 * Manages the logging and writing of logs to a log file. This is implemented as a singleton.
	 */
	public class Logger {
		
		// this would also be the name of the log file
		private string name;
		private string logFilePath;
		
		private Queue<Log> bufferQueue; // the primary queue where logs would be enqueued
		private Queue<Queue<Log>> writeQueue; // this is the work queue
		
		private bool currentlyWriting;
		
		private static int WRITE_TIME_INTERVAL = 5;
		private CountdownTimer writeTimer;
		
		private static Logger ONLY_INSTANCE = null;
		
		public static Logger GetInstance() {
			if(ONLY_INSTANCE == null) {
				ONLY_INSTANCE = new Logger();
			}
			
			return ONLY_INSTANCE;
		}
		
		private Logger() {
			bufferQueue = new Queue<Log>();
			writeQueue = new Queue<Queue<Log>>();
			currentlyWriting = false;
			
			writeTimer = new CountdownTimer(WRITE_TIME_INTERVAL);
		}
		
		/**
		 * Sets the name.
		 */
		public void SetName(string name) {
			this.name = name;
			this.logFilePath = Application.persistentDataPath + "/" + name + "Log.txt";
			Debug.Log("logFilePath: " + logFilePath);
		}
		
		/**
		 * Logs a normal message.
		 */
		public void Log(string message) {
			Log(LogLevel.NORMAL, message);
		}
		
		/**
		 * Logs a warning message.
		 */
		public void LogWarning(string message) {
			Log(LogLevel.WARNING, message);
		}
		
		/**
		 * Logs an error message.
		 */
		public void LogError(string message) {
			Log(LogLevel.ERROR, message);
		}
		
		/**
		 * Adds a log with a specified level.
		 */
		public void Log(LogLevel level, string message) {
			if(level == LogLevel.NORMAL) {
				Debug.Log(message);
			} else if(level == LogLevel.WARNING) {
				Debug.LogWarning(message);
			} else if(level == LogLevel.ERROR) {
				Debug.LogError(message);
			}
			
			bufferQueue.Enqueue(new Log(level, message));
		}
		
		private void EnqueueWriteTask() {
			Queue<Log> logsToWriteQueue = new Queue<Log>();
			while(bufferQueue.Count > 0) {
				Log log = bufferQueue.Dequeue();
				logsToWriteQueue.Enqueue(log);
			}
			
			writeQueue.Enqueue(logsToWriteQueue);
		}
		
		/**
		 * Writes the remaining logs. Usually called when the application is exiting.
		 */
		public void WriteRemainingLogs() {
			EnqueueWriteTask();
			
			// wait for all logs to be written
			while(writeQueue.Count > 0) {
				ProcessTaskQueue();
			}
		}
		
		/**
		 * Update routines of the logger.
		 */
		public void Update() {
			if(currentlyWriting) {
				// still writing
				return;
			}
			
			if(bufferQueue.Count == 0) {
				// nothing to write
				return;
			}
			
			writeTimer.Update();
			if(writeTimer.HasElapsed()) {
				EnqueueWriteTask();
				Action action = ProcessTaskQueue;
				UnityThreadHelper.CreateThread(action);
				writeTimer.Reset();
			}
		}
		
		/**
		 * Writes the latest log queue to the file.
		 */
		private void ProcessTaskQueue() {
#if UNITY_WEBPLAYER
			// we don't support logging since System.IO.File.AppendText() is not supported in web player
#else
			currentlyWriting = true;
			
			// check if file already exists
			Assertion.Assert(!string.IsNullOrEmpty(logFilePath), "logFilePath should not be empty. Try setting a name for the log.");
			StreamWriter writer = null;
			try {
				if(File.Exists(logFilePath)) {
					writer = File.AppendText(logFilePath);
				} else {
					writer = File.CreateText(logFilePath);
				}
				
				Queue<Log> frontLogQueue = writeQueue.Dequeue();
				while(frontLogQueue.Count > 0) {
					Log log = frontLogQueue.Dequeue();
					string logLine = log.Level.Name + ": " + log.Timestamp.ToString() + " " + log.Message;
					writer.WriteLine(logLine);
				}
			} finally {
				writer.Flush();
				writer.Close();
			}
			
			currentlyWriting = false;
#endif
		}
		
		/**
		 * Returns the log file path.
		 */
		public string LogFilePath {
			get{
				return logFilePath;
			}
		}
	}
}

