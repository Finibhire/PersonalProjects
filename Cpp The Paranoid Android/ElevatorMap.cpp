#include "ElevatorMap.h"
#include <set>


ElevatorMap::ElevatorMap(const unsigned int exitLevel, const unsigned int exitPos, const unsigned int createEleCount)
{
	Levels = exitLevel + 1;
	elevators = new std::set<Elevator>[Levels];
	elevators[exitLevel].insert(*(new Elevator(exitLevel, exitPos, Exit)));
}


ElevatorMap::~ElevatorMap()
{
	delete[] elevators;
}


bool ElevatorMap::AddElevator(const unsigned int level, const unsigned int position)
{
	if (level < 0 || level >= Levels)
	{
		return false;
	}

	Elevator* n = new Elevator(level, position, PreFabricated);
	std::set<Elevator>::iterator it = elevators[level].find(*n);

	if (it == elevators[level].end())
	{
		elevators[level].insert(*n);
	}
	else if (it->Type == PotentialCreate)
	{
		elevators[level].erase(it);
		elevators[level].insert(*n);
	}
	
	return true;
}