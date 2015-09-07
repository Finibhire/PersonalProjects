#include "BinarySearch.h"
#include <climits>
#include <stdexcept>

/// FindFirst method varients will always out perform FindFast when n > 11585 
/// In general though, FindFast will only show better results when n < 1176
/// the difference is maximized @ n = 8 resulting in a difference of 19 operations
/// difference @ n = 500 is 7 opperations

BinarySearch::BinarySearch()
{
}


BinarySearch::~BinarySearch()
{
}

/// This method does a binary search on the passed in array looking for the key value.  Because this method shortcuts to
/// completion if the key is found early on average this reduces the number of cycle operations by 2 (to a minimum of 1).
/// typename Value must have overloaded >, <, =
/// Operations -
///     best case opperations  = 4 + FindFastUnSafeMidNoParamCheck  = 4 + 11 * (log2(n) - 2)
///     worst case opperations = 5 + FindFastSafeMidNoParamCheck    = 5 + 13 * (log2(n) - 2)
/// returns The an index found in values[] that is equal to the key
///         If length == 0 then the bitwise inverse of 0 is returned (comes before the 0'th element)
///         If the key doesn't exist in values[] then 
///           the bitwise inverse of the next highest key value that exists in values[] is returned 
///           or if the value is greater that the key at values[imax] then the bitwise inverse of length is returned 
///         *The returned value is not garrentteed to be the first index in values[] that is equal to the key.
template <typename Key, typename Value, int length>
inline int BinarySearch::FindFast(const Key key, const Value(&values)[length])
{
	if (length == 0)
	{
		return ~((int)0);
	}
	unsigned int imax = length - 1;
	if (imax >= UINT_MAX / 2)
	{
		return BinarySearch::FindFastSafeMidNoParamCheck(key, values, 0, imax);
	}
	return BinarySearch::FindFastUnSafeMidNoParamCheck(key, values, 0, imax);
}

/// This method does a binary search on the passed in array looking for the key value.  Because this method shortcuts to
/// completion if the key is found early on average this reduces the number of cycle operations by 2 (to a minimum of 1).
/// typename Value must have overloaded >, <, =
/// Operations -
///     best case opperations  = 4 + FindFastUnSafeMidNoParamCheck  = 4 + 11 * (log2(n) - 2)
///     worst case opperations = 5 + FindFastSafeMidNoParamCheck    = 5 + 13 * (log2(n) - 2)
/// returns The an index found in values[] that is equal to the key
///         If length == 0 && imin == 0 then the bitwise inverse of 0 is returned (comes before the 0'th element)
///         If the key doesn't exist in values[] then 
///           the bitwise inverse of the next highest key value that exists in values[] is returned 
///           or if the value is greater that the key at values[imax] then the bitwise inverse of length is returned 
///         *The returned value is not garrentteed to be the first index in values[] that is equal to the key.
template <typename Key, typename Value, int length>
inline int BinarySearch::FindFast(const Key key, const Value(&values)[length], unsigned int imin)
{
	if (imin >= length)
	{
		if (length == 0 && imin == 0)
		{
			return ~((int)0);
		}
		throw std::range_error("imin >= length");
	}
	unsigned int imax = length - 1;
	if (imax >= UINT_MAX / 2)
	{
		return BinarySearch::FindFastSafeMidNoParamCheck(key, values, imin, imax);
	}
	return BinarySearch::FindFastUnSafeMidNoParamCheck(key, values, imin, imax);
}

/// This method does a binary search on the passed in array looking for the key value.  Because this method shortcuts to
/// completion if the key is found early on average this reduces the number of cycle operations by 2 (to a minimum of 1).
/// typename Value must have overloaded >, <, =
/// Operations -
///     best case opperations  = 4 + FindFastUnSafeMidNoParamCheck  = 4 + 11 * (log2(n) - 2)
///     worst case opperations = 5 + FindFastSafeMidNoParamCheck    = 5 + 13 * (log2(n) - 2)
/// returns The an index found in values[] that is equal to the key
///         If length == 0 && imin == 0 then the bitwise inverse of 0 is returned (comes before the 0'th element)
///         If the key doesn't exist in values[] then 
///           the bitwise inverse of the next highest key value that exists in values[] is returned 
///           or if the value is greater that the key at values[imax] then the bitwise inverse of (imax + 1) is returned 
///         *The returned value is not garrentteed to be the first index in values[] that is equal to the key.
template <typename Key, typename Value, int length>
inline int BinarySearch::FindFast(const Key key, const Value (&values)[length], unsigned int imin, unsigned int imax)
{
	if (imin > imax)
	{
		throw std::range_error("imin > imax");
	}
	if (imin >= length || imax >= length)
	{
		if (length == 0 && imin == 0)
		{
			return ~((int)0);
		}
		throw std::range_error("imin >= length or imax >= length");
	}
	if (imax >= UINT_MAX / 2)
	{
		return BinarySearch::FindFastSafeMidNoParamCheck(key, values, imin, imax);
	}
	return BinarySearch::FindFastUnSafeMidNoParamCheck(key, values, imin, imax);
}

/// This method provides the most optimized fastest returns for BinarySearch at the expense of parameter validation.
/// Slightly slower than FindFastUnSafeMidNoParamCheck().
/// Remarks -
///    This method does not validate parameters
/// Safe Use Constraints - 
///    typename Value must have overloaded operators >, <, =
///    imin < values.length
///    imax < values.length
/// Operations -
///     best case opperations  = 12 * (log2(n) - 2)
///     worst case opperations = 13 * (log2(n) - 2)
///     On average this method will out perform FindFirst when n < 2048
///     FindFirst will always outperform this method when n > 11585
template <typename Key, typename Value>
inline int BinarySearch::FindFastSafeMidNoParamCheck(const Key key, const Value values[], unsigned int imin, unsigned int imax)
{
	while (imax >= imin)
	{
		int imid = imin + ((imax - imin) / 2);

		if (values[imid] < key)
		{
			imin = imid + 1;
		}
		else if (values[imid] > key)
		{
			imax = imid - 1;
		}
		else
		{
			return imid;
		}
	}

	return ~imin;
}

/// This method provides the most optimized fastest returns for BinarySearch at the expense of parameter validation.
/// Slightly faster than FindFastSafeMidNoParamCheck().
/// Remarks -
///    This method does not validate parameters
/// Safe Use Constraints - 
///    typename Value must have overloaded operators >, <, =
///    imin < values.length
///    imax < values.length
///    imax + imax - 1 <= UINT_MAX  // provides a nominal performace improvement of reducing a single addition per log2(n) cycle
/// Operations -
///     best case opperations  = 11 * (log2(n) - 2)
///     worst case opperations = 12 * (log2(n) - 2)
///     On average this method will out perform FindFirst when n < 1176
///     FindFirst will always outperform this method when n > 5793
template <typename Key, typename Value>
inline int BinarySearch::FindFastUnSafeMidNoParamCheck(const Key key, const Value values[], unsigned int imin, unsigned int imax)
{
	while (imax >= imin)
	{
		int imid = (imin + imax) / 2;

		if (values[imid] < key)
		{
			imin = imid + 1;
		}
		else if (values[imid] > key)
		{
			imax = imid - 1;
		}
		else
		{
			return imid;
		}
	}

	return ~imin;
}

///////////////////////////////////////// - FindFirst - /////////////////////////////////////////

/// This method does a binary search on the passed in array looking for the key value.  Because this method shortcuts to
/// completion if the key is found early on average this reduces the number of cycle operations by 2 (to a minimum of 1).
/// typename Value must have overloaded >, <, =
/// Operations -
///     best case opperations  = 4 + FindFirstUnSafeMidNoParamCheck  = 5 + 9 * log2(n)
///     worst case opperations = 5 + FindFirstSafeMidNoParamCheck    = 7 + 10 * log2(n)
/// returns The an index found in values[] that is equal to the key
///         If length == 0 then the bitwise inverse of 0 is returned (comes before the 0'th element)
///         If the key doesn't exist in values[] then 
///           the bitwise inverse of the next highest key value that exists in values[] is returned 
///           or if the value is greater that the key at values[imax] then the bitwise inverse of length is returned 
///         *The returned value is not garrentteed to be the first index in values[] that is equal to the key.
template <typename Key, typename Value, int length>
inline int BinarySearch::FindFirst(const Key key, const Value(&values)[length])
{
	if (length == 0)
	{
		return ~((int)0);
	}
	unsigned int imax = length - 1;
	if (imax >= UINT_MAX / 2)
	{
		return BinarySearch::FindFirstSafeMidNoParamCheck(key, values, 0, imax);
	}
	return BinarySearch::FindFirstUnSafeMidNoParamCheck(key, values, 0, imax);
}

/// This method does a binary search on the passed in array looking for the key value.  Because this method shortcuts to
/// completion if the key is found early on average this reduces the number of cycle operations by 2 (to a minimum of 1).
/// typename Value must have overloaded >, <, =
/// Operations -
///     best case opperations  = 4 + FindFirstUnSafeMidNoParamCheck  = 5 + 9 * log2(n)
///     worst case opperations = 5 + FindFirstSafeMidNoParamCheck    = 7 + 10 * log2(n)
/// returns The an index found in values[] that is equal to the key
///         If length == 0 && imin == 0 then the bitwise inverse of 0 is returned (comes before the 0'th element)
///         If the key doesn't exist in values[] then 
///           the bitwise inverse of the next highest key value that exists in values[] is returned 
///           or if the value is greater that the key at values[imax] then the bitwise inverse of length is returned 
///         *The returned value is not garrentteed to be the first index in values[] that is equal to the key.
template <typename Key, typename Value, int length>
inline int BinarySearch::FindFirst(const Key key, const Value(&values)[length], unsigned int imin)
{
	if (imin >= length)
	{
		if (length == 0 && imin == 0)
		{
			return ~((int)0);
		}
		throw std::range_error("imin >= length");
	}
	unsigned int imax = length - 1;
	if (imax >= UINT_MAX / 2)
	{
		return BinarySearch::FindFirstSafeMidNoParamCheck(key, values, imin, imax);
	}
	return BinarySearch::FindFirstUnSafeMidNoParamCheck(key, values, imin, imax);
}

/// This method does a binary search on the passed in array looking for the key value.  Because this method shortcuts to
/// completion if the key is found early on average this reduces the number of cycle operations by 2 (to a minimum of 1).
/// typename Value must have overloaded >, <, =
/// Operations -
///     best case opperations  = 4 + FindFirstUnSafeMidNoParamCheck  = 6 + 9 * log2(n)
///     worst case opperations = 5 + FindFirstSafeMidNoParamCheck    = 8 + 10 * log2(n)
/// returns The an index found in values[] that is equal to the key
///         If length == 0 && imin == 0 then the bitwise inverse of 0 is returned (comes before the 0'th element)
///         If the key doesn't exist in values[] then 
///           the bitwise inverse of the next highest key value that exists in values[] is returned 
///           or if the value is greater that the key at values[imax] then the bitwise inverse of (imax + 1) is returned 
///         *The returned value is not garrentteed to be the first index in values[] that is equal to the key.
template <typename Key, typename Value, int length>
inline int BinarySearch::FindFirst(const Key key, const Value(&values)[length], unsigned int imin, unsigned int imax)
{
	if (imin > imax)
	{
		throw std::range_error("imin > imax");
	}
	if (imin >= length || imax >= length)
	{
		if (length == 0 && imin == 0)
		{
			return ~((int)0);
		}
		throw std::range_error("imin >= length or imax >= length");
	}
	if (imax >= UINT_MAX / 2)
	{
		return BinarySearch::FindFirstSafeMidNoParamCheck(key, values, imin, imax);
	}
	return BinarySearch::FindFirstUnSafeMidNoParamCheck(key, values, imin, imax);
}

/// This method provides the most optimized Firstest returns for BinarySearch at the expense of parameter validation.
/// Slightly slower than FindFirstUnSafeMidNoParamCheck().
/// Remarks -
///    This method does not validate parameters
/// Safe Use Constraints - 
///    typename Value must have overloaded operators >, <, =
///    imin < values.length
///    imax < values.length
/// Operations -
///     best case opperations  = 10 * log2(n) + 2
///     worst case opperations = 10 * log2(n) + 3
///     On average this method will out perform FindFast when n > 2048
///     This method will always outperform FindFast when n > 11585
template <typename Key, typename Value>
inline int BinarySearch::FindFirstSafeMidNoParamCheck(const Key key, const Value values[], unsigned int imin, unsigned int imax)
{
	while (imin < imax)
	{
		int imid = imin + ((imax - imin) / 2);

		if (values[imid] >= key)
		{
			imax = imid;
		}
		else
		{
			imin = imid + 1;
		}
	}

	if (values[imin] == key)
	{
		return imin;
	}
	return ~(imax + 1);
}

/// This method provides the most optimized Firstest returns for BinarySearch at the expense of parameter validation.
/// Slightly Faster than FindFirstSafeMidNoParamCheck().
/// Remarks -
///    This method does not validate parameters
/// Safe Use Constraints - 
///    typename Value must have overloaded operators >, <, =
///    imin <= imax
///    imin < values.length
///    imax < values.length
///    imax + imax - 1 <= UINT_MAX  // provides a nominal performace improvement of reducing a single addition per log2(n) cycle
/// Operations -
///     best case opperations  = 9 * log2(n) + 2
///     worst case opperations = 9 * log2(n) + 3
///     On average this method will out perform FindFast when n > 1176
///     This method will always outperform FindFast when n > 5793
template <typename Key, typename Value>
inline int BinarySearch::FindFirstUnSafeMidNoParamCheck(const Key key, const Value values[], unsigned int imin, unsigned int imax)
{
	while (imin < imax)
	{
		int imid = (imin + imax) / 2;

		if (values[imid] >= key)
		{
			imax = imid;
		}
		else
		{
			imin = imid + 1;
		}
	}

	if (values[imin] == key)
	{
		return imin;
	}
	return ~(imax + 1);
}