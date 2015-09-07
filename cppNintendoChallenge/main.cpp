
#include <iostream>

#include <sstream>

using namespace std;

int main()
{
	int size;
	cin >> size;



	unsigned int* a = new unsigned int[size / 16]; // <- input tab to encrypt
	unsigned int* b = new unsigned int[size / 16]; // <- output tab

	for (int i = 0; i < size / 16; i++) {   // Read size / 16 integers to a
		cin >> hex >> a[i];
	}

	for (int i = 0; i < size / 16; i++) {   // Write size / 16 zeros to b
		b[i] = 0;
	}

	string ii, jj;

	for (int i = 0; i < size; i++)
	{
		stringstream ss;
		ss << i;
		ss >> ii;
		ss.clear();
		for (int j = 0; j < size; j = j + 1)
		{
			ss << j;
			ss >> jj;
			ss.clear();
			b[(i + j) / 32] ^= ((a[i / 32] >> (i % 32)) &
				(a[j / 32 + size / 32] >> (j % 32)) & 1) << ((i + j) % 32);   // Magic centaurian operation
		}
	}

	for (int i = 0; i < size / 16; i++)
		cout << hex << b[i] << " ";       // print result


	cin >> size;
	/*
	Good luck humans
	*/
	return 0;
}
