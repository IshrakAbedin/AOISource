include "globals.mzn";

int: num_work; % MOD
int: num_nrg; % MOD
int: num_act = num_work + num_nrg;
int: num_slot; % MOD
int: starting_energy; % MOD

set of int: activities = 1..num_act;
set of int: slots = 1..num_slot;
set of int: works = 1..num_work;
set of int: nrgs = (num_work + 1)..num_act;

array[activities] of var int: energy; %MOD
array[works] of var int: work_duration; % MOD
array[works] of var 0..9: priority; % MOD

array[slots] of var activities: schedule;
array[slots] of var int: energy_consumed;
var int: score;

% Priority wise enforces task to be done in order and the efficiency is kept in score
constraint score = sum(i in slots where schedule[i] <= num_work)(priority[schedule[i]] * (num_slot - i));

% Cumulative energy count calculation
constraint forall(i in slots)(
  energy_consumed[i] = starting_energy + sum(j in slots where j <= i)(energy[schedule[j]])
);

% Enforces never to work with negetive energy
constraint forall(i in slots)(
  energy_consumed[i] >= 0
);

% Enforces all the works to be finished
constraint forall(i in works)(
  count(schedule, i) = work_duration[i]
);

% Enforces all type of recreational activities to appear atleast once
constraint forall(i in nrgs)(
  count(schedule, i) >= 1
);

% Target is to finish all the necessary tasks first based on priority to maximize score
solve maximize score;

output["\(schedule)"];