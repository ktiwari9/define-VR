# importing the required modules 
import matplotlib.pyplot as plt 
import pandas as pd
import numpy as np
import json
import sys
import os
import time
import glob

## SPECIAL FUNCTIONS, SET THE VARIABLES TO TRUE TO USE
fullmode = False		# Set this manually to true to plot absolutely every mean variation available
minmax = False		# Set this manually to true to print out the best and worst (total) scorers
fitline = False		# Fits line(s) and returns the slope of it

# Plots trajectory, used in other functions
def plot_path(filepath):
	df = pd.read_csv(filepath)
	x = df["pos_x"]
	y = df["pos_z"]
	plt.plot(x, y)
	plt.ion()


# Plot the trajectories of each trial into a single plot
def plot_session_trajectories(folder, savepath):
	filelist = get_trajectory_files(folder)
	plt.figure()
	for file in filelist:
		plot_path(file)
	plt.title("Trajectories")
	plt.ylabel("Z-position")
	plt.xlabel("X-position")
	plt.savefig(os.path.join(savepath, "trajectories.png"))

def getMinMax(scenariofolder, label, scorekey="",block=""):
	currentMin = -1
	currentMax = -1
	minParticipant = ""
	maxParticipant = ""
	filelist = get_result_files(scenariofolder,scorekey)
	for file in filelist:
		df = pd.read_csv(file)
		score = df[label]
		blocks = df["block_num"]
		
		#remove scores of wrong block
		for i in range(0, score.shape[0]):
			if(block != str(blocks[i]) and block != ""):
				score.pop(i)
				
		tot = np.sum(score)
		if(tot < currentMin or currentMin == -1):
			currentMin = tot
			minParticipant = df["ppid"][0]
		if(tot > currentMax or currentMax == -1): 
			currentMax = tot
			maxParticipant = df["ppid"][0]
			
	print("Max scoring participant: "+maxParticipant+"\nMin scoring participant: "+minParticipant+"\n")

# Plots dual
def plot_dual_mean(scenariofolder, savepath, label,block=""):
	filelist1 = get_result_files(scenariofolder,"speed")
	filelist2 = get_result_files(scenariofolder,"accuracy")
	scores1 = []
	scores2 = []
	plt.figure()
	for file in filelist1:
		df = pd.read_csv(file)
		score = df[label]
		blocks = df["block_num"]
		for i in range(0, score.shape[0]):
			if(block != str(blocks[i]) and block != ""):
				score.pop(i)
		scores1.append(score)
	means1 = np.nanmean(scores1, 0)
	std1 = np.nanstd(scores1,0)
	
	for file in filelist2:
		df = pd.read_csv(file)
		score = df[label]
		blocks = df["block_num"]
		for i in range(0, score.shape[0]):
			if(block != str(blocks[i]) and block != ""):
				score.pop(i)
		scores2.append(score)
	means2 = np.nanmean(scores2, 0)
	std2 = np.nanstd(scores2,0)
	
	x = np.arange(1,len(means1)+1)
	
	plt.errorbar(x-0.1,means1,std1, linestyle='None', fmt='-o',capsize=3)
	plt.ion()
	plt.errorbar(x+0.1,means2,std2, linestyle='None', fmt='-o',capsize=3)
	plt.ion()
	plt.axvline(x=15, linewidth=1, color='b', linestyle=':')
	plt.ylabel(label)
	plt.xlabel("Trial number")
	plt.legend(("Block change","Time Group","Accuracy Group"))
	plt.title("Average "+label+"s per trial")
	plt.savefig(os.path.join(savepath, "average"+label+"s_dual.png"))

# Plots the average score for each trial in the set
def plot_mean(scenariofolder, savepath, label,scorekey="",block=""):
	filelist = get_result_files(scenariofolder,scorekey)
	scores = []
	plt.figure()
	for file in filelist:
		df = pd.read_csv(file)
		score = df[label]
		blocks = df["block_num"]
		for i in range(0, score.shape[0]):			
			if(block != str(blocks[i]) and block != ""):
				score.pop(i)
		scores.append(score)
		
	means = np.nanmean(scores, 0)
	std = np.nanstd(scores, 0)
	x = range(1,len(means)+1)
	
	# Plot
	if(scorekey == ""):
		plt.title("Average "+label+" per trial")
	else:
		plt.title("Average "+label+" per trial, "+scorekey+" weighted scoring")
	plt.ylabel(label)
	plt.xlabel("Trial number")
	plt.errorbar(x,means,std, linestyle='None', fmt='-o', capsize=3)
	if(scorekey == ""):
		scorekey = "allmethods"
	if(block == ""):
		plt.ion()
		plt.axvline(x=15, linewidth=1, color='b', linestyle=':')
		block = "fullsession"
	elif(block == "1"):
		block = "practice"
	elif(block == "2"):
		block = "testing"
	plt.savefig(os.path.join(savepath, "average"+label+"s_"+scorekey+"_"+block+".png"))


# Fits a line
def fit_line(scenariofolder, label,scorekey="",block=""):
	filelist = get_result_files(scenariofolder,scorekey)
	k = []
	plt.figure()
	for file in filelist:
		df = pd.read_csv(file)
		score = df[label]
		blocks = df["block_num"]
		for i in range(0, score.shape[0]):
			if(block != str(blocks[i]) and block != ""):
				score.pop(i)
		x = range(1,len(score)+1)
		k.append(np.polyfit(x, score,1)[0])
		
	means = np.nanmean(k, 0)
	std = np.nanstd(k,0)
	print(k)
	print("\nMean K: " + str(means) + "\nSTD of K: " + str(std) + "\n")
	

# Gets and returns all the result files found within the given path for non-bad sessions
def get_result_files(folder,scorekey=""):
	unfilteredlist = glob.glob(folder+'/**/trial_results.csv', recursive=True)
	filteredlist = []
	for file in unfilteredlist:
		sessionpath = os.path.dirname(file)
		notes = json.loads(open(os.path.join(sessionpath, "notes.json"),'r').read())
		if( not notes["session_marked_as_bad"]):
			scoresettings = json.loads(open(os.path.join(sessionpath, "scoreSettings.json"), 'r').read())
			if(scoresettings["keyword"] == scorekey or scorekey == ""):
				filteredlist.append(file)
	return filteredlist
	
# Gets and returns all the result files found within the given path for non-bad sessions
def get_trajectory_files(folder):
	unfilteredlist = glob.glob(folder+'/**/player_movement_T*.csv', recursive=True)
	filteredlist = []
	for file in unfilteredlist:
		sessionpath = os.path.dirname(file)
		notes = json.load(open(os.path.join(sessionpath, "notes.json"),'r').read())
		if( not notes["session_marked_as_bad"]):
			filteredlist.append(file)
	return filteredlist

# Plots every iteration of mean available
def fullplot(folderpath, savepath):
	opt1 = ["score","distance","time"]
	opt2 = ["","speed","accuracy"]
	opt3 = ["","1","2"]
	for o1 in opt1:
		for o2 in opt2:
			for o3 in opt3:
				plot_mean(folderpath, savepath, o1, o2, o3)

def main():	
	folderpath = ""		# Path where to look for experiment data
	savepath = ""		# Path where to save the created plots
	label = "" 			# the name of variable to plot in y axis (ie. score, time, distance)
	
	# Verify that we were given a valid experiment data folder
	try:
		folderpath = sys.argv[1]
	except:
		folderpath = input("Where to look for experiment data? [current folder]\n")
		if(folderpath == ""):
			folderpath = "."
	if(not os.path.exists(folderpath)):
		print("No such directory, quitting.")
		time.sleep(1) # sleep so user can read the above message
		return
	
	# Try to obtain savefolder, defaults to data folder if not given
	try:
		savepath = sys.argv[2]
	except:
		savepath = input("Where to save created plots? [current folder]\n")
		if(savepath == ""):
			savepath = "."
		if(not os.path.exists(savepath)):
			print("No such directory, quitting.")
			time.sleep(1) # sleep so user can read the above message
			return
	
	if(fullmode):
		fullplot(folderpath, savepath)
		return
	
	# Try to obtain label
	try:
		label = sys.argv[3]
	except:
		label = input("What to plot? [1]\n  1 = Mean score\n  2 = Mean distance\n  3 = Mean time\n  4 = Trajectories\n")
		if(label == ""):
			label = "1"
		if(label == "1" or label.lower() == "score"):
			label = "score"
		elif(label == "2" or label.lower() == "distance"):
			label = "distance"
		elif(label == "3" or label.lower() == "time"):
			label = "time"
		elif(label == "4" or label.lower() == "trajectory"):
			label = "trajectory"
		else:
			print("Invalid instruction! Quitting.")
			time.sleep(1)
			return

	# Try to obtain scorekey
	try:
		scorekey = sys.argv[4]
	except:
		scorekey = input("Process only specific score methods? [0]\n  0 = All methods\n  1 = Only Method 1\n  2 = Only Method 2\n  3 = Dual\n")
		if(scorekey == "" or scorekey == "0" or scorekey.lower() == "all methods"):
			scorekey = ""
		elif(scorekey == "1" or scorekey.lower() == "method 1"):
			scorekey = "speed"
		elif(scorekey == "2" or scorekey.lower() == "method 2"):
			scorekey = "accuracy"
		elif(scorekey == "3" or scorekey.lower() == "dual"):
			scorekey = "dual"
			
	# Try to obtain block
	try:
		block = sys.argv[5]
	except:
		block = input("Process only specific blocks? [0]\n  0 = All blocks\n  1 = Only practice block\n  2 = Only test block\n")
		if(block == "" or block == "0" or block.lower() == "all"):
			block = ""
		elif(block == "1" or block.lower() == "practice"):
			block = "1"
		elif(block == "2" or block.lower() == "method 2"):
			block = "2"
	
	if(minmax):
		getMinMax(folderpath, label, scorekey, block)
		return
	if(fitline):
		fit_line(folderpath, label, scorekey, block)
		return
	
	if(label == "trajectory"):
		plot_session_trajectories(folderpath, savepath, block)
	else:
		if(scorekey == "dual"):
			plot_dual_mean(folderpath, savepath, label, block)
		else:
			plot_mean(folderpath, savepath, label, scorekey, block)
	
	
	input("Press [enter] to exit and close all plots.\n")
	
if __name__ == "__main__":
    main()