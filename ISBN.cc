#include "ISBN.h"

int ISBN::Input_Data( char Input[129] )
{
	int i, TypeCnt = 0, DataCnt = 0, N;

	for ( i=0;i<128;i++ )
	{
		Data[i] = Input[i];

		if ( Input[i] == '\0' )
			break;
	}

	Length = i;

	if ( Length != 13 )
		return 0;

	for ( i=0;i<Length;i++ )
	{
		if ( ( '0' > Data[i] || Data[i] > '9' ) && Data[i] != 'X' && Data[i] != '-' )
			return 0;

		if ( Data[i] == '-' )
		{
			switch( TypeCnt )
			{
			case 0:
				if ( DataCnt > 5 || !DataCnt )
					return 0;
			break;

			case 1:
				if ( DataCnt > 7 || !DataCnt )
					return 0;
			break;

			case 2:
				if ( DataCnt > 6 || !DataCnt )
					return 0;
			break;
			}

			TypeCnt ++;

			DataCnt = 0;

			continue;
		}

		DataCnt++;
	}

	if ( DataCnt > 1 || !DataCnt )
		return 0;

	return 1;
}

int ISBN::Calc_Data()
{
	int CheckSumNum = 10, Sum = 0;

	for ( int i=0;i<Length - 1;i++ )
	{
		if ( Data[i] == '-' )
			continue;

		Sum += ( Data[i] - '0' ) * CheckSumNum--;
	}

	if ( ( 11 - Sum%11 ) % 11 == Data[ Length - 1] - 'X' + 10 )
		return 1;

	if ( ( 11 - Sum%11 ) % 11 == Data[ Length - 1 ] - '0' )
		return 1;
	
	return 0;
}