import pymzn
from sys import argv
from prettytable import PrettyTable
import webbrowser
from os import path
import flattime

def format_mzn_solution(solution : pymzn.mzn.minizinc) -> list:
    """Extracts solution from raw string and then pushes each of the elements in a list"""
    solution = str(solution)
    index = solution.find('schedule') + 12
    processed_solution = ''
    for char in solution[index : -1]:
        if char == ']':
            break
        else:
            processed_solution = processed_solution + char
    solution_list = processed_solution.split(',')
    for i in range(len(solution_list)):
        solution_list[i] = int(solution_list[i])
    return solution_list

def input_system() -> (list, dict, int, str):
    """Takes input from user if no commandline arguments are passed"""
    print('Enter your inputs')
    works = []
    recrs = []
    duration = []
    max_duration = []
    priority = []
    work_energy = []
    recr_energy = []
    starting_energy = 100
    quanta = 15
    slots = 20
    time = 0
    stime = ''
    etime = ''
    while(True):
        inputs = input().lower().split()
        if(inputs[0] == 'work' or inputs[0] == 'wrk'):
            works.append(inputs[1])
            work_energy.append(-1 * int(inputs[2]))
            duration.append(int(inputs[3]))
            max_duration.append(int(inputs[4]))
            priority.append(int(inputs[5]))
        elif(inputs[0] == 'recr' or inputs[0] == 'rec'):
            recrs.append(inputs[1])
            recr_energy.append(int(inputs[2]))
        elif(inputs[0] == 'nrg' or inputs[0] == 'ener' or inputs[0] == 'energy' or inputs[0] == 'enrg'):
            starting_energy = int(inputs[1])
        elif(inputs[0] == 'quanta' or inputs[0] == 'qnta'):
            quanta = int(inputs[1])
        elif(inputs[0] == 'time'):
            time = int(inputs[1])
        elif(inputs[0] == 'stime'):
            stime = inputs[1]
        elif(inputs[0] == 'etime'):
            etime = inputs[1]
        elif(inputs[0] == 'run'):
            break
        elif(inputs[0] == 'clear'):
            works = []
            recrs = []
            duration = []
            max_duration = []
            priority = []
            work_energy = []
            recr_energy = []
            starting_energy = 100
            quanta = 15
            slots = 20
            time = 0
        else:
            show_help()
    
    activities = works + recrs
    energies = work_energy + recr_energy
    if(time == 0):
        time = flattime.get_flat_difference(stime, etime)
    slots = int(time / quanta)
    for i in range(len(duration)):
        duration[i] = int(duration[i] / quanta)
        if duration[i] <= 0:
            duration[i] = 1
    for i in range(len(max_duration)):
        max_duration[i] = int(max_duration[i] / quanta)
        if max_duration[i] <= 0:
            max_duration[i] = 1
        if max_duration[i] < duration[i]:
            max_duration[i] = duration[i]
    for i in range(len(duration)):
        energies[i] = int(energies[i] / duration[i])

    data = {}
    data['num_work'] = len(works)
    data['num_nrg'] = len(recrs)
    data['num_slot'] = slots
    data['starting_energy'] = starting_energy
    data['energy'] = energies
    data['work_duration'] = duration
    data['work_max_duration'] = max_duration
    data['priority'] = priority

    return activities, data, quanta, stime

def read_system(inputfile : str) -> (list, dict, int, str):
    """Reads inputs from inputfile if commandline argument is passed"""
    doc = open(inputfile, 'r')
    lines = doc.readlines()
    doc.close()
    works = []
    recrs = []
    duration = []
    max_duration=[]
    priority = []
    work_energy = []
    recr_energy = []
    starting_energy = 100
    quanta = 15
    slots = 20
    time = 0
    stime = ''
    etime = ''
    for line in lines:
        inputs = line.lower().split()
        if(inputs[0] == 'work' or inputs[0] == 'wrk'):
            works.append(inputs[1])
            work_energy.append(-1 * int(inputs[2]))
            duration.append(int(inputs[3]))
            max_duration.append(int(inputs[4]))
            priority.append(int(inputs[5]))
        elif(inputs[0] == '#' or inputs[0] == '%'):
            continue
        elif(inputs[0] == 'recr' or inputs[0] == 'rec'):
            recrs.append(inputs[1])
            recr_energy.append(int(inputs[2]))
        elif(inputs[0] == 'nrg' or inputs[0] == 'ener' or inputs[0] == 'energy' or inputs[0] == 'enrg'):
            starting_energy = int(inputs[1])
        elif(inputs[0] == 'quanta' or inputs[0] == 'qnta'):
            quanta = int(inputs[1])
        elif(inputs[0] == 'time'):
            time = int(inputs[1])
        elif(inputs[0] == 'stime'):
            stime = inputs[1]
        elif(inputs[0] == 'etime'):
            etime = inputs[1]
        elif(inputs[0] == 'run'):
            break
        elif(inputs[0] == 'clear'):
            works = []
            recrs = []
            duration = []
            max_duration = []
            priority = []
            work_energy = []
            recr_energy = []
            starting_energy = 100
            quanta = 15
            slots = 20
            time = 0
        else:
            show_help()
    
    activities = works + recrs
    energies = work_energy + recr_energy
    time = flattime.get_flat_difference(stime, etime)
    slots = int(time / quanta)
    for i in range(len(duration)):
        duration[i] = int(duration[i] / quanta)
        if duration[i] <= 0:
            duration[i] = 1
    for i in range(len(max_duration)):
        max_duration[i] = int(max_duration[i] / quanta)
        if max_duration[i] <= 0:
            max_duration[i] = 1
        if max_duration[i] < duration[i]:
            max_duration[i] = duration[i]
    for i in range(len(duration)):
        energies[i] = int(energies[i] / duration[i])

    data = {}
    data['num_work'] = len(works)
    data['num_nrg'] = len(recrs)
    data['num_slot'] = slots
    data['starting_energy'] = starting_energy
    data['energy'] = energies
    data['work_duration'] = duration
    data['work_max_duration'] = max_duration
    data['priority'] = priority

    return activities, data, quanta, stime

def comprehensive_output(tasklist : list, quanta : int):
    """Output system without using prettytable"""
    if(len(tasklist) == 0):
        print('No task found')
        return
    lasttask = None
    counter = 0
    for task in tasklist:
        if(lasttask == None):
            lasttask = task
            counter = 1
        elif(lasttask != task):
            print('-', lasttask, '\t', quanta * counter, 'minutes')
            lasttask = task
            counter = 1
        else:
            counter += 1
    print('-', lasttask, '\t', quanta * counter, 'minutes')

def tabular_output(tasklist : list, quanta : int, stime : str):
    """Output system with prettytable and then html formatted to show using a web browser"""
    etime = ''
    if(len(tasklist) == 0):
        print('No task found')
        return
    table = PrettyTable(['Task', 'Starting Time', 'Ending Time', 'Duration'])
    lasttask = None
    counter = 0
    for task in tasklist:
        if(lasttask == None):
            lasttask = task
            counter = 1
        elif(lasttask != task):
            etime = flattime.add_flat_to_time12(stime, quanta * counter)
            table.add_row([str(lasttask).capitalize(), stime, etime, str(quanta * counter) + ' minutes'])
            stime = etime
            lasttask = task
            counter = 1
        else:
            counter += 1
    etime = flattime.add_flat_to_time12(stime, quanta * counter)
    table.add_row([str(lasttask).capitalize(), stime, etime, str(quanta * counter) + ' minutes'])
    stime = etime
    html = table.get_html_string(attributes={"id":"schedule"})
    print(table)
    part = open('resultPart.rpt', 'r')
    output = part.read()
    part.close()
    part = open('resultPart2.rpt', 'r')
    output2 = part.read()
    part.close()
    output = output + '\n' + html + output2
    out = open('schedule.html', 'w+')
    out.write(output)
    out.close()
    webbrowser.open('file://' + path.realpath('schedule.html'))

def show_help():
    """Function to show help"""
    print('Please read ReadMe.md or Manual.txt')

def main():
    """Main driver function"""
    activities = []
    data = {}
    quanta = 0
    stime = ''
    if(len(argv) == 2 and str(argv[1]).find('.txt') > -1): # If commandline arguments are detected, read input from the given input file
        activities, data, quanta, stime = read_system(str(argv[1]))
    else: # If commandline arguments are not found, ask user to give input
        activities, data, quanta, stime = input_system()
    print('Please wait while an AI solver tries to manage your schedule ... ...')
    try: # Tries to run the model with given data using CoinBC solver
        result = pymzn.minizinc('Routine.mzn', 'Null.dzn', data = data, solver = pymzn.cbc)
        schedule = format_mzn_solution(result)
    except(Exception):
        print('Sorry solution cannot be found')
        print('Press ENTER to exit ...')
        input()
        return
    tasklist = []
    for task in schedule:
        tasklist.append(activities[task - 1]) # Prepares a task list
    # comprehensive_output(tasklist, quanta)
    tabular_output(tasklist, quanta, stime) # Displays output
    return

main()