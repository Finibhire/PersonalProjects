#pragma once
#include <set>

enum ElevatorType : int
{
	PreFabricated = 1, Entrance = 2, Exit = 3, PotentialCreate = 4
};

struct Elevator
{
public:
	unsigned int Level, Position;
	ElevatorType Type;

	Elevator(const unsigned int level, const unsigned int position, const ElevatorType type)
	{
		Level = level;
		Position = position;
		Type = type;
	}

	inline bool operator<(const Elevator& other) const
	{
		return (Position < other.Position);
	}
};

class ElevatorMap
{
private:
	unsigned int Levels;
	std::set<Elevator> *elevators;
public:
	ElevatorMap(const unsigned int exitLevel, const unsigned int exitPos, const unsigned int createEleCount);
	~ElevatorMap();

	bool AddElevator(const unsigned int level, const unsigned int position);
};

