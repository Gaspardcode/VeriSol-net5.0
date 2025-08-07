if [ $# -eq 0 ]; then
	echo 'Usage <dir> [-p for ratio]'
	exit 1;
fi

echo $#

if [ $# -eq 2 ]; then
	ttl=0
	for file in $(ls $1/*.cs ); do
		cnt=$(cat $file | wc -l )
		ttl=$(( $cnt + $ttl ))
	done
	for file in $(ls $1/*.cs ); do
		cnt=$(cat $file | wc -l )
		echo $file has ~$(( $cnt * 100 / $ttl))% of the code lines in this folder
	done
else
	ttl=0
	for file in $(ls $1/*.cs ); do
		cnt=$(cat $file | wc -l )
		echo $file has $cnt lines of code
		ttl=$(( $cnt + $ttl ))
	done
fi
echo Total : $ttl lines of code. Good luck buddy.
