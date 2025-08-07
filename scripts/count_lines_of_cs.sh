a=0
for file in $(ls $1/*.cs ); do
 	a=$(( $(cat $file | wc -l ) + $a ))
done
echo $a lines of code. Good luck buddy.
