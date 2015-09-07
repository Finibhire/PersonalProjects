#pragma once
class BinarySearch
{
public:
	BinarySearch();
	~BinarySearch();
	template <typename Key, typename Value, int length>
	inline int FindFast(const Key key, const Value(&values)[length]);
	template <typename Key, typename Value, int length>
	inline int FindFast(const Key key, const Value(&values)[length], unsigned int imin);
	template <typename Key, typename Value, int length>
	inline int FindFast(const Key key, const Value(&values)[length], unsigned int imin, unsigned int imax);
	template <typename Key, typename Value>
	inline int FindFastSafeMidNoParamCheck(const Key key, const Value values[], unsigned int imin, unsigned int imax);
	template <typename Key, typename Value>
	inline int FindFastUnSafeMidNoParamCheck(const Key key, const Value values[], unsigned int imin, unsigned int imax);

	template <typename Key, typename Value, int length>
	inline int FindFirst(const Key key, const Value(&values)[length]);
	template <typename Key, typename Value, int length>
	inline int FindFirst(const Key key, const Value(&values)[length], unsigned int imin);
	template <typename Key, typename Value, int length>
	inline int FindFirst(const Key key, const Value(&values)[length], unsigned int imin, unsigned int imax);
	template <typename Key, typename Value>
	inline int FindFirstSafeMidNoParamCheck(const Key key, const Value values[], unsigned int imin, unsigned int imax);
	template <typename Key, typename Value>
	inline int FindFirstUnSafeMidNoParamCheck(const Key key, const Value values[], unsigned int imin, unsigned int imax);
};

