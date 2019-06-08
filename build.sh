command -v Unity 1>/dev/null || exit 1
command -v docker 1>/dev/null || exit 1

Unity -batchmode -executeMethod WebGLBuilder.build -projectPath ./Client -nographics -quit

sudo docker build -t projekt-php-2019 .
