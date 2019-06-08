<?php
declare (strict_types = 1);

abstract class Method
{
    const Get = 'GET';
    const Post = 'POST';
}

class Route
{
    public $method;
    public $path;
    public $callback;

    public function __construct(string $method, string $path, callable $callback)
    {
        $this->method = $method;
        $this->path = splitPath($path);
        $this->callback = $callback;
    }
}

class Router
{
    private $routes = array();

    public function process()
    {
        $reqHeaders = apache_request_headers();        
        error_log(var_export($reqHeaders, true));
        if (!isset($reqHeaders['Authorization']) || !$this->isAuthorized($reqHeaders['Authorization'])) {
            header('Content-type:application/json;charset=utf-8');
            echo json_encode(array(
                'Success' => false,
                'Error' => 'NotAuthorized'
            ));
            return;
        }

        $path = splitPath($_SERVER['REQUEST_URI']);
        $method = $_SERVER['REQUEST_METHOD'];
        foreach ($this->routes as $route) {
            if ($this->pathFits($path, $route->path) && $route->method === $method) {
                header('Content-type:application/json;charset=utf-8');
                $res = call_user_func($route->callback, ...$this->getPathVariables($path, $route->path));
                echo json_encode($res);
                return;
            }
        }

        header("HTTP/1.0 404 Not Found");
    }

    private function isAuthorized($auth)
    {
        if (empty($auth)) return false;
        if (!substr($auth, 0, 4) === 'Basic') return false;
        $creds = explode(':', base64_decode(substr($auth, 6)));
        if ($creds[0] === $creds[1]) {
            return true;
        }
    }

    private function pathFits(array $reqPath, array $routePath)
    {
        if (count($reqPath) !== count($routePath)) return false;

        for ($i = 0; $i < count($routePath); $i++) {
            $part = $routePath[$i];
            if ($this->isPathVariable($part)) {
                continue;
            } else {
                if ($part !== $reqPath[$i]) {
                    return false;
                }
            }
        }

        return true;
    }

    private function getPathVariables(array $reqPath, array $routePath)
    {
        $res = array();
        for ($i = 0; $i < count($routePath); $i++) {
            $part = $routePath[$i];
            if ($this->isPathVariable($part)) {
                $res[] = $reqPath[$i];
            }
        }
        return $res;
    }

    private function isPathVariable(string $part)
    {
        $first = substr($part, 0, 1);
        $last = substr($part, strlen($part) - 1, 1);
        return $first === '{' && $last === '}';
    }

    public function get(string $path, callable $callback)
    {
        $this->routes[] = new Route(Method::Get, $path, $callback);
    }
}

function splitPath(string $path)
{
    return array_values(array_filter(explode('/', $path), function ($e) {
        return $e !== '';
    }));
}
