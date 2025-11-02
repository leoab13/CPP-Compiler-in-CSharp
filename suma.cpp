#include <stdio.h>
#include <math.h>

int radio, altura, i, j; // Variables de tipo entero
float x, y, z;
char c, d, e;


void main()
{
    if (1 != 2)
    {
        printf("\nIngrese el valor de d = \n");
        scanf("%f", &d);
        if (d % 2 != 0)
        {

            for (i = 0; i < d; i++)
            {
                printf("-");
            }
            printf("\n");

            i = d;
            do
            {
                printf(" - ", radio);
                i--;
            } while (i > 0);
            printf("\n");

            i = 0;
            while (i < d)
            {
                for (j = 0; j <= i; j++)
                {
                    if (j % 2 == 0){
                        printf("+");
                    }
                    else{
                        printf("-");
                    }
                }
                printf("\n");
                i++;
            }
        }
        else
        {
            printf("El valor de d debe ser impar.\n");
        }
    }
}

