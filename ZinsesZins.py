"""
Compound Interest calculator

interest given ->> daily

@author: tomigorn
@date: 25.01.2022
"""

### ### ### ### ### ### ### ### ###
### CHANGE HERE:
### ### ### ### ### ### ### ### ###
initial_investment = 150
monthly_contribution = 50
interest_rate = 1.0+(0.1/100.0)
number_of_years = 10
### ### ### ### ### ### ### ### ###
### END OF CHANGE
### ### ### ### ### ### ### ### ###

days_per_month = 30
days_per_year = 365

total_contribution = initial_investment
balance = initial_investment

"""
function to format numbers into financial format
input: float number
output: string
"""


def format_financial_number(input_number):
    output_string = input_number
    output_string = format(output_string, ".1f")
    output_string = float(output_string)
    output_string = format(output_string, ",").replace(",","'")
    output_string = format(output_string, ">9")
    output_string = "$" + output_string
    return output_string


def format_percentage_number(input_number):
    output_string = input_number
    output_string = format(output_string, ".1f")
    output_string = float(output_string)
    output_string = format(output_string, ",").replace(",","'")
    return output_string


#business logic of compound interest calculation
days_spent = 0
total_days = number_of_years*days_per_year
while days_spent < total_days:
    days_current_month = 0
    while days_current_month < days_per_month:
        balance *= interest_rate
        days_current_month += 1
        days_spent += 1
    balance += monthly_contribution
    total_contribution += monthly_contribution
    
#print results
print(number_of_years, "years later...")
print("\nfuture value:")
print(format_financial_number(balance))
print("\ntotal contributions:")
print(format_financial_number(total_contribution))
print("\n", format_percentage_number(100 * balance / total_contribution), "% gain")
print("\ninterest earned")
print(format_financial_number(balance - total_contribution))


#EXAMPLE
#input:
"""
initial_investment = 150
monthly_contribution = 50
interest_rate = 1.0+(0.1/100.0)
number_of_years = 10
"""
#output:
"""
10 years later...

future value:
$ 67'893.9

total contributions:
$  6'250.0

 1'086.3 % gain

interest earned
$ 61'643.9


** Process exited - Return Code: 0 **
Press Enter to exit terminal
"""
